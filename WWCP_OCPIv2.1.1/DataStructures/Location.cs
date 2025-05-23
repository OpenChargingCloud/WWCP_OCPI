﻿/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
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

using System.Text;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_1_1.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// The charging location is a group of EVSEs at more or less the same geographical location
    /// and operated by the same charge point operator.
    /// 
    /// Typically a charging location is the exact location of the group
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

        #region Data

        private readonly Lock patchLock = new();

        #endregion

        #region Properties

        /// <summary>
        /// The parent CommonAPI of this charging location.
        /// </summary>
        internal  CommonAPI?                          CommonAPI                { get; set; }

        /// <summary>
        /// The ISO-3166 alpha-2 country code of the charge point operator that 'owns' this charging location.
        /// </summary>
        [Mandatory]
        public    CountryCode                         CountryCode              { get; }

        /// <summary>
        /// The identification of the charge point operator that 'owns' this charging location (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public    Party_Id                            PartyId                  { get; }

        /// <summary>
        /// The identification of the charging location within the CPOs platform (and suboperator platforms).
        /// This field can never be changed, modified or renamed.
        /// </summary>
        [Mandatory]
        public    Location_Id                         Id                       { get; }

        /// <summary>
        /// The general type of the charging location.
        /// </summary>
        [Mandatory]
        public    LocationType                        LocationType             { get; }

        /// <summary>
        /// The optional display name of the charging location.
        /// string(255)
        /// </summary>
        [Optional]
        public    String?                             Name                     { get; }

        /// <summary>
        /// The address of the charging location.
        /// string(45)
        /// </summary>
        [Mandatory]
        public    String                              Address                  { get; }

        /// <summary>
        /// The city or town of the charging location.
        /// string(45)
        /// </summary>
        [Mandatory]
        public    String                              City                     { get; }

        /// <summary>
        /// The postal code of the charging location.
        /// string(10)
        /// </summary>
        [Mandatory]
        public    String                              PostalCode               { get; }

        /// <summary>
        /// The country of the charging location.
        /// </summary>
        [Mandatory]
        public    Country                             Country                  { get; }

        /// <summary>
        /// The geographical location of this charging location.
        /// </summary>
        [Mandatory]
        public    GeoCoordinate                       Coordinates              { get; }

        /// <summary>
        /// The optional enumeration of additional geographical locations of related geo coordinates that might be relevant to the EV driver.
        /// </summary>
        [Optional]
        public    IEnumerable<AdditionalGeoLocation>  RelatedLocations         { get; }

        /// <summary>
        /// The optional enumeration of Electric Vehicle Supply Equipments (EVSE) at this charging location.
        /// </summary>
        [Optional]
        public    IEnumerable<EVSE>                   EVSEs
            => evsesByUId.Values;

        /// <summary>
        /// The enumeration of all internal EVSE (unique) identifications at this charging location.
        /// </summary>
        [Mandatory]
        public    IEnumerable<EVSE_UId>               EVSEUIds
            => evsesByUId.Keys;

        /// <summary>
        /// The optional enumeration of all EVSE identifications at this charging location.
        /// </summary>
        [Optional]
        public    IEnumerable<EVSE_Id>                EVSEIds
            => evsesById.Keys;

        /// <summary>
        /// The optional enumeration of human-readable directions on how to reach the location.
        /// </summary>
        [Optional]
        public    IEnumerable<DisplayText>            Directions               { get; }

        /// <summary>
        /// Optional information about the charging station operator.
        /// </summary>
        /// <remarks>When not specified, the information retrieved from the credentials module matching the country_code and party_id should be used instead.</remarks>
        [Optional]
        public    BusinessDetails?                    Operator                 { get; }

        /// <summary>
        /// Optional information about the suboperator.
        /// </summary>
        [Optional]
        public    BusinessDetails?                    SubOperator              { get; }

        /// <summary>
        /// Optional information about the owner.
        /// </summary>
        [Optional]
        public    BusinessDetails?                    Owner                    { get; }

        /// <summary>
        /// The optional enumeration of facilities this charging location directly belongs to.
        /// </summary>
        [Optional]
        public    IEnumerable<Facilities>             Facilities               { get; }

        /// <summary>
        /// One of IANA tzdata’s TZ-values representing the time zone of the charging location (http://www.iana.org/time-zones).
        /// </summary>
        /// <example>"Europe/Oslo", "Europe/Zurich"</example>
        [Optional]
        public    String?                             Timezone                 { get; }

        /// <summary>
        /// The optional times when the EVSEs at the charging location can be accessed for charging.
        /// </summary>
        [Optional]
        public    Hours?                              OpeningTimes             { get; }

        /// <summary>
        /// Indicates if the EVSEs are still charging outside the opening
        /// hours of the charging location. E.g. when the parking garage closes its
        /// barriers over night, is it allowed to charge till the next
        /// morning? [Default: true]
        /// </summary>
        [Optional]
        public    Boolean?                            ChargingWhenClosed       { get; }

        /// <summary>
        /// The optional enumeration of images related to the charging location such as photos or logos.
        /// </summary>
        [Optional]
        public    IEnumerable<Image>                  Images                   { get; }

        /// <summary>
        /// Optional details on the energy supplied at this charging location.
        /// </summary>
        [Optional]
        public    EnergyMix?                          EnergyMix                { get; }

        /// <summary>
        /// Whether this charging location may be published on an website or app etc., or not.
        /// </summary>
        [Optional, VendorExtension("PlugSurfing")]
        public    Boolean?                            Publish                  { get; }

        public JObject                                CustomData               { get; }
        public UserDefinedDictionary                  InternalData             { get; }


        /// <summary>
        /// The timestamp when this charging location was created.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
        public    DateTime                            Created                  { get; }

        /// <summary>
        /// The timestamp when this charging location was last updated (or created).
        /// </summary>
        [Mandatory]
        public    DateTime                            LastUpdated              { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this location.
        /// </summary>
        [Optional, VendorExtension(VE.GraphDefined, "OCPIComputerScienceExtension")]
        public    String                              ETag                     { get; private set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging location.
        /// </summary>
        /// <param name="CountryCode">An ISO-3166 alpha-2 country code of the charge point operator that 'owns' this charging location.</param>
        /// <param name="PartyId">An identification of the charge point operator that 'owns' this charging location (following the ISO-15118 standard).</param>
        /// <param name="Id">An identification of the charging location within the CPOs platform (and suboperator platforms).</param>
        /// <param name="LocationType">An optional general type of parking at the charging location.</param>
        /// <param name="Address">The address of the charging location.</param>
        /// <param name="City">The city or town of the charging location.</param>
        /// <param name="PostalCode">The postal code of the charging location.</param>
        /// <param name="Country">The country of the charging location.</param>
        /// <param name="Coordinates">The geographical location of this charging location.</param>
        /// 
        /// <param name="Name">An optional display name of the charging location.</param>
        /// <param name="RelatedLocations">An optional enumeration of additional geographical locations of related geo coordinates that might be relevant to the EV driver.</param>
        /// <param name="EVSEs">An optional enumeration of Electric Vehicle Supply Equipments (EVSE) at this charging location.</param>
        /// <param name="Directions">An optional enumeration of human-readable directions on how to reach the location.</param>
        /// <param name="Operator">Optional information about the charging station operator.</param>
        /// <param name="SubOperator">Optional information about the suboperator.</param>
        /// <param name="Owner">Optional information about the owner.</param>
        /// <param name="Facilities">An optional enumeration of facilities this charging location directly belongs to.</param>
        /// <param name="Timezone">One of IANA tzdata’s TZ-values representing the time zone of the charging location (http://www.iana.org/time-zones).</param>
        /// <param name="OpeningTimes">An optional times when the EVSEs at the charging location can be accessed for charging.</param>
        /// <param name="ChargingWhenClosed">Indicates if the EVSEs are still charging outside the opening hours of the charging location.</param>
        /// <param name="Images">An optional enumeration of images related to the charging location such as photos or logos.</param>
        /// <param name="EnergyMix">Optional details on the energy supplied at this charging location.</param>
        /// 
        /// <param name="Publish">Whether this charging location may be published on an website or app etc., or not.</param>
        /// 
        /// <param name="Created">An optional timestamp when this charging location was created.</param>
        /// <param name="LastUpdated">An optional timestamp when this charging location was last updated (or created).</param>
        /// 
        /// <param name="CustomLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomAdditionalGeoLocationSerializer">A delegate to serialize custom additional geo location JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomEnergyMeterSerializer">A delegate to serialize custom energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomHoursSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        public Location(CountryCode                                                   CountryCode,
                        Party_Id                                                      PartyId,
                        Location_Id                                                   Id,
                        LocationType                                                  LocationType,
                        String                                                        Address,
                        String                                                        City,
                        String                                                        PostalCode,
                        Country                                                       Country,
                        GeoCoordinate                                                 Coordinates,

                        String?                                                       Name                                         = null,
                        IEnumerable<AdditionalGeoLocation>?                           RelatedLocations                             = null,
                        IEnumerable<EVSE>?                                            EVSEs                                        = null,
                        IEnumerable<DisplayText>?                                     Directions                                   = null,
                        BusinessDetails?                                              Operator                                     = null,
                        BusinessDetails?                                              SubOperator                                  = null,
                        BusinessDetails?                                              Owner                                        = null,
                        IEnumerable<Facilities>?                                      Facilities                                   = null,
                        String?                                                       Timezone                                     = null,
                        Hours?                                                        OpeningTimes                                 = null,
                        Boolean?                                                      ChargingWhenClosed                           = null,
                        IEnumerable<Image>?                                           Images                                       = null,
                        EnergyMix?                                                    EnergyMix                                    = null,

                        // Non-Standard extensions
                        Boolean?                                                      Publish                                      = null,

                        JObject?                                                      CustomData                                   = null,
                        UserDefinedDictionary?                                        InternalData                                 = null,

                        DateTime?                                                     Created                                      = null,
                        DateTime?                                                     LastUpdated                                  = null,
                        EMSP_Id?                                                      EMSPId                                       = null,

                        CustomJObjectSerializerDelegate<Location>?                    CustomLocationSerializer                     = null,
                        CustomJObjectSerializerDelegate<AdditionalGeoLocation>?       CustomAdditionalGeoLocationSerializer        = null,
                        CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                        CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                        CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                        CustomJObjectSerializerDelegate<EnergyMeter<Location>>?       CustomLocationEnergyMeterSerializer          = null,
                        CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?           CustomEVSEEnergyMeterSerializer              = null,
                        CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                        CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                        CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null,
                        CustomJObjectSerializerDelegate<BusinessDetails>?             CustomBusinessDetailsSerializer              = null,
                        CustomJObjectSerializerDelegate<Hours>?                       CustomHoursSerializer                        = null,
                        CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null,
                        CustomJObjectSerializerDelegate<EnergyMix>?                   CustomEnergyMixSerializer                    = null)

            : this(null,
                   CountryCode,
                   PartyId,
                   Id,
                   LocationType,
                   Address,
                   City,
                   PostalCode,
                   Country,
                   Coordinates,

                   Name,
                   RelatedLocations,
                   EVSEs,
                   Directions,
                   Operator,
                   SubOperator,
                   Owner,
                   Facilities,
                   Timezone,
                   OpeningTimes,
                   ChargingWhenClosed,
                   Images,
                   EnergyMix,

                   Publish,

                   CustomData,
                   InternalData,

                   Created,
                   LastUpdated,
                   EMSPId,
                   CustomLocationSerializer,
                   CustomAdditionalGeoLocationSerializer,
                   CustomEVSESerializer,
                   CustomStatusScheduleSerializer,
                   CustomConnectorSerializer,
                   CustomLocationEnergyMeterSerializer,
                   CustomEVSEEnergyMeterSerializer,
                   CustomTransparencySoftwareStatusSerializer,
                   CustomTransparencySoftwareSerializer,
                   CustomDisplayTextSerializer,
                   CustomBusinessDetailsSerializer,
                   CustomHoursSerializer,
                   CustomImageSerializer,
                   CustomEnergyMixSerializer)

        { }


        /// <summary>
        /// Create a new charging location.
        /// </summary>
        /// <param name="CountryCode">An ISO-3166 alpha-2 country code of the charge point operator that 'owns' this charging location.</param>
        /// <param name="PartyId">An identification of the charge point operator that 'owns' this charging location (following the ISO-15118 standard).</param>
        /// <param name="Id">An identification of the charging location within the CPOs platform (and suboperator platforms).</param>
        /// <param name="LocationType">An optional general type of parking at the charging location.</param>
        /// <param name="Address">The address of the charging location.</param>
        /// <param name="City">The city or town of the charging location.</param>
        /// <param name="PostalCode">The postal code of the charging location.</param>
        /// <param name="Country">The country of the charging location.</param>
        /// <param name="Coordinates">The geographical location of this charging location.</param>
        /// 
        /// <param name="Name">An optional display name of the charging location.</param>
        /// <param name="RelatedLocations">An optional enumeration of additional geographical locations of related geo coordinates that might be relevant to the EV driver.</param>
        /// <param name="EVSEs">An optional enumeration of Electric Vehicle Supply Equipments (EVSE) at this charging location.</param>
        /// <param name="Directions">An optional enumeration of human-readable directions on how to reach the location.</param>
        /// <param name="Operator">Optional information about the charging station operator.</param>
        /// <param name="SubOperator">Optional information about the suboperator.</param>
        /// <param name="Owner">Optional information about the owner.</param>
        /// <param name="Facilities">An optional enumeration of facilities this charging location directly belongs to.</param>
        /// <param name="Timezone">One of IANA tzdata’s TZ-values representing the time zone of the charging location (http://www.iana.org/time-zones).</param>
        /// <param name="OpeningTimes">An optional times when the EVSEs at the charging location can be accessed for charging.</param>
        /// <param name="ChargingWhenClosed">Indicates if the EVSEs are still charging outside the opening hours of the charging location.</param>
        /// <param name="Images">An optional enumeration of images related to the charging location such as photos or logos.</param>
        /// <param name="EnergyMix">Optional details on the energy supplied at this charging location.</param>
        /// 
        /// <param name="Publish">Whether this charging location may be published on an website or app etc., or not.</param>
        /// 
        /// <param name="Created">An optional timestamp when this charging location was created.</param>
        /// <param name="LastUpdated">An optional timestamp when this charging location was last updated (or created).</param>
        /// 
        /// <param name="CustomLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomAdditionalGeoLocationSerializer">A delegate to serialize custom additional geo location JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomLocationEnergyMeterSerializer">A delegate to serialize custom location energy meter JSON objects.</param>
        /// 
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomHoursSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        public Location(CommonAPI?                                                    CommonAPI,
                        CountryCode                                                   CountryCode,
                        Party_Id                                                      PartyId,
                        Location_Id                                                   Id,
                        LocationType                                                  LocationType,
                        String                                                        Address,
                        String                                                        City,
                        String                                                        PostalCode,
                        Country                                                       Country,
                        GeoCoordinate                                                 Coordinates,

                        String?                                                       Name                                         = null,
                        IEnumerable<AdditionalGeoLocation>?                           RelatedLocations                             = null,
                        IEnumerable<EVSE>?                                            EVSEs                                        = null,
                        IEnumerable<DisplayText>?                                     Directions                                   = null,
                        BusinessDetails?                                              Operator                                     = null,
                        BusinessDetails?                                              SubOperator                                  = null,
                        BusinessDetails?                                              Owner                                        = null,
                        IEnumerable<Facilities>?                                      Facilities                                   = null,
                        String?                                                       Timezone                                     = null,
                        Hours?                                                        OpeningTimes                                 = null,
                        Boolean?                                                      ChargingWhenClosed                           = null,
                        IEnumerable<Image>?                                           Images                                       = null,
                        EnergyMix?                                                    EnergyMix                                    = null,

                        // Non-Standard extensions
                        Boolean?                                                      Publish                                      = null,

                        JObject?                                                      CustomData                                   = null,
                        UserDefinedDictionary?                                        InternalData                                 = null,

                        DateTime?                                                     Created                                      = null,
                        DateTime?                                                     LastUpdated                                  = null,
                        EMSP_Id?                                                      EMSPId                                       = null,

                        CustomJObjectSerializerDelegate<Location>?                    CustomLocationSerializer                     = null,
                        CustomJObjectSerializerDelegate<AdditionalGeoLocation>?       CustomAdditionalGeoLocationSerializer        = null,
                        CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                        CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                        CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                        CustomJObjectSerializerDelegate<EnergyMeter<Location>>?       CustomLocationEnergyMeterSerializer          = null,
                        CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?           CustomEVSEEnergyMeterSerializer              = null,
                        CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                        CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                        CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null,
                        CustomJObjectSerializerDelegate<BusinessDetails>?             CustomBusinessDetailsSerializer              = null,
                        CustomJObjectSerializerDelegate<Hours>?                       CustomHoursSerializer                        = null,
                        CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null,
                        CustomJObjectSerializerDelegate<EnergyMix>?                   CustomEnergyMixSerializer                    = null)

        {

            this.CommonAPI            = CommonAPI;
            this.CountryCode          = CountryCode;
            this.PartyId              = PartyId;
            this.Id                   = Id;
            this.LocationType         = LocationType;
            this.Address              = Address;
            this.City                 = City;
            this.PostalCode           = PostalCode;
            this.Country              = Country;
            this.Coordinates          = Coordinates;

            this.Name                 = Name;
            this.RelatedLocations     = RelatedLocations?.Distinct() ?? Array.Empty<AdditionalGeoLocation>();
            this.Directions           = Directions?.      Distinct() ?? Array.Empty<DisplayText>();
            this.Operator             = Operator;
            this.SubOperator          = SubOperator;
            this.Owner                = Owner;
            this.Facilities           = Facilities?.      Distinct() ?? Array.Empty<Facilities>();
            this.Timezone             = Timezone;
            this.OpeningTimes         = OpeningTimes;
            this.ChargingWhenClosed   = ChargingWhenClosed;
            this.Images               = Images?.          Distinct() ?? Array.Empty<Image>();
            this.EnergyMix            = EnergyMix;

            // Non-Standard extensions
            this.Publish              = Publish;

            this.CustomData           = CustomData                   ?? [];
            this.InternalData         = InternalData                 ?? new UserDefinedDictionary();

            this.Created              = Created                      ?? LastUpdated ?? Timestamp.Now;
            this.LastUpdated          = LastUpdated                  ?? Created     ?? Timestamp.Now;

            if (EVSEs is not null && EVSEs.Any())
            {
                foreach (var evse in EVSEs.Distinct())
                {

                    evse.ParentLocation = this;

                    evsesByUId.TryAdd(evse.UId,
                                      evse);

                    if (evse.EVSEId.HasValue)
                        evsesById.TryAdd(evse.EVSEId.Value,
                                         evse);

                }
            }

            this.ETag                 = CalcSHA256Hash(EMSPId,
                                                       CustomLocationSerializer,
                                                       CustomAdditionalGeoLocationSerializer,
                                                       CustomEVSESerializer,
                                                       CustomStatusScheduleSerializer,
                                                       CustomConnectorSerializer,
                                                       CustomLocationEnergyMeterSerializer,
                                                       CustomEVSEEnergyMeterSerializer,
                                                       CustomTransparencySoftwareStatusSerializer,
                                                       CustomTransparencySoftwareSerializer,
                                                       CustomDisplayTextSerializer,
                                                       CustomBusinessDetailsSerializer,
                                                       CustomHoursSerializer,
                                                       CustomImageSerializer,
                                                       CustomEnergyMixSerializer);

            unchecked
            {

                hashCode = CountryCode.        GetHashCode()        * 97 ^
                           PartyId.            GetHashCode()        * 89 ^
                           Id.                 GetHashCode()        * 83 ^
                           LocationType.       GetHashCode()        * 79 ^
                           Address.            GetHashCode()        * 73 ^
                           City.               GetHashCode()        * 71 ^
                           PostalCode.         GetHashCode()        * 67 ^
                           Country.            GetHashCode()        * 61 ^
                           Coordinates.        GetHashCode()        * 59 ^
                           Created.            GetHashCode()        * 53 ^
                           LastUpdated.        GetHashCode()        * 47 ^

                          (Name?.              GetHashCode()  ?? 0) * 43 ^
                          (RelatedLocations?.  GetHashCode()  ?? 0) * 41 ^
                          (EVSEs?.             CalcHashCode() ?? 0) * 37 ^
                          (Directions?.        GetHashCode()  ?? 0) * 31 ^
                          (Operator?.          GetHashCode()  ?? 0) * 29 ^
                          (SubOperator?.       GetHashCode()  ?? 0) * 23 ^
                          (Owner?.             GetHashCode()  ?? 0) * 19 ^
                          (Facilities?.        GetHashCode()  ?? 0) * 17 ^
                          (Timezone?.          GetHashCode()  ?? 0) * 13 ^
                          (OpeningTimes?.      GetHashCode()  ?? 0) * 11 ^
                          (ChargingWhenClosed?.GetHashCode()  ?? 0) *  7 ^
                          (Images?.            GetHashCode()  ?? 0) *  5 ^
                          (EnergyMix?.         GetHashCode()  ?? 0) *  3 ^

                          (Publish?.           GetHashCode()  ?? 0);

            }

        }

        #endregion


        #region (static) Parse   (JSON, CountryCodeURL = null, PartyIdURL = null, LocationIdURL = null, CustomLocationParser = null)

        /// <summary>
        /// Parse the given JSON representation of an Location.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="LocationIdURL">An optional location identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomLocationParser">A delegate to parse custom location JSON objects.</param>
        public static Location Parse(JObject                                 JSON,
                                     CountryCode?                            CountryCodeURL         = null,
                                     Party_Id?                               PartyIdURL             = null,
                                     Location_Id?                            LocationIdURL          = null,
                                     CustomJObjectParserDelegate<Location>?  CustomLocationParser   = null)
        {

            if (TryParse(JSON,
                         out var location,
                         out var errorResponse,
                         CountryCodeURL,
                         PartyIdURL,
                         LocationIdURL,
                         CustomLocationParser))
            {
                return location!;
            }

            throw new ArgumentException("The given JSON representation of a location is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Location, out ErrorResponse, LocationIdURL = null, CustomLocationParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an Location.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Location">The parsed location.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                             JSON,
                                       [NotNullWhen(true)]  out Location?  Location,
                                       [NotNullWhen(false)] out String?    ErrorResponse)

            => TryParse(JSON,
                        out Location,
                        out ErrorResponse,
                        null,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an Location.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Location">The parsed location.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="LocationIdURL">An optional location identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomLocationParser">A delegate to parse custom location JSON objects.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out Location?      Location,
                                       [NotNullWhen(false)] out String?        ErrorResponse,
                                       CountryCode?                            CountryCodeURL         = null,
                                       Party_Id?                               PartyIdURL             = null,
                                       Location_Id?                            LocationIdURL          = null,
                                       CustomJObjectParserDelegate<Location>?  CustomLocationParser   = null)
        {

            try
            {

                Location = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CountryCode           [optional, internal]

                if (JSON.ParseOptional("country_code",
                                       "country code",
                                       CountryCode.TryParse,
                                       out CountryCode? CountryCodeBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!CountryCodeURL.HasValue && !CountryCodeBody.HasValue)
                {
                    ErrorResponse = "The country code is missing!";
                    return false;
                }

                if (CountryCodeURL.HasValue && CountryCodeBody.HasValue && CountryCodeURL.Value != CountryCodeBody.Value)
                {
                    ErrorResponse = "The optional country code given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse PartyIdURL            [optional, internal]

                if (JSON.ParseOptional("party_id",
                                       "party identification",
                                       Party_Id.TryParse,
                                       out Party_Id? PartyIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!PartyIdURL.HasValue && !PartyIdBody.HasValue)
                {
                    ErrorResponse = "The party identification is missing!";
                    return false;
                }

                if (PartyIdURL.HasValue && PartyIdBody.HasValue && PartyIdURL.Value != PartyIdBody.Value)
                {
                    ErrorResponse = "The optional party identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse Id                    [optional]

                if (JSON.ParseOptional("id",
                                       "location identification",
                                       Location_Id.TryParse,
                                       out Location_Id? LocationIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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

                #region Parse LocationType          [mandatory]

                if (!JSON.ParseMandatory("type",
                                         "location type",
                                         OCPIv2_1_1.LocationType.TryParse,
                                         out LocationType LocationType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Address               [mandatory]

                if (!JSON.ParseMandatoryText("address",
                                             "address",
                                             out String Address,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse City                  [mandatory]

                if (!JSON.ParseMandatoryText("city",
                                             "city",
                                             out String City,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PostalCode            [mandatory]

                if (!JSON.ParseMandatoryText("postal_code",
                                             "postal code",
                                             out String PostalCode,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Country               [mandatory]

                if (!JSON.ParseMandatory("country",
                                         "country",
                                         org.GraphDefined.Vanaheimr.Illias.Country.TryParse,
                                         out Country Country,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Coordinates           [mandatory]

                if (!JSON.ParseMandatoryJSON("coordinates",
                                             "geo coordinates",
                                             GeoCoordinate.TryParse,
                                             out GeoCoordinate Coordinates,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Name                  [optional]

                var Name = JSON.GetString("name");

                #endregion

                #region Parse RelatedLocations      [optional]

                if (JSON.ParseOptionalHashSet("related_locations",
                                              "related locations",
                                              AdditionalGeoLocation.TryParse,
                                              out HashSet<AdditionalGeoLocation> RelatedLocations,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EVSEs                 [optional]

                if (JSON.ParseOptionalHashSet("evses",
                                              "evses",
                                              EVSE.TryParse,
                                              out HashSet<EVSE> EVSEs,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Directions            [optional]

                if (JSON.ParseOptionalHashSet("directions",
                                              "multi-language directions",
                                              DisplayText.TryParse,
                                              out HashSet<DisplayText> Directions,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Operator              [optional]

                if (JSON.ParseOptionalJSON("operator",
                                           "operator",
                                           BusinessDetails.TryParse,
                                           out BusinessDetails Operator,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Suboperator           [optional]

                if (JSON.ParseOptionalJSON("suboperator",
                                           "suboperator",
                                           BusinessDetails.TryParse,
                                           out BusinessDetails Suboperator,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Owner                 [optional]

                if (JSON.ParseOptionalJSON("owner",
                                           "owner",
                                           BusinessDetails.TryParse,
                                           out BusinessDetails Owner,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Facilities            [optional]

                if (JSON.ParseOptionalHashSet("facilities",
                                              "facilities",
                                              OCPIv2_1_1.Facilities.TryParse,
                                              out HashSet<Facilities> Facilities,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TimeZone              [optional]

                var TimeZone = JSON.GetString("time_zone");

                #endregion

                #region Parse OpeningTimes          [optional]

                if (JSON.ParseOptionalJSON("opening_times",
                                           "opening times",
                                           Hours.TryParse,
                                           out Hours OpeningTimes,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ChargingWhenClosed    [optional]

                if (JSON.ParseOptional("charging_when_closed",
                                       "charging when closed",
                                       out Boolean? ChargingWhenClosed,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Images                [optional]

                if (JSON.ParseOptionalHashSet("images",
                                              "images",
                                              Image.TryParse,
                                              out HashSet<Image> Images,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EnergyMix             [optional]

                if (JSON.ParseOptionalJSON("energy_mix",
                                           "energy mix",
                                           OCPIv2_1_1.EnergyMix.TryParse,
                                           out EnergyMix EnergyMix,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                // Non-Standard extensions

                #region Parse Publish               [optional]

                if (JSON.ParseOptional("publish",
                                       "publish",
                                       out Boolean? Publish,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Parse Created               [optional, NonStandard]

                if (JSON.ParseOptional("created",
                                       "created",
                                       out DateTime? Created,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LastUpdated           [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTime LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                Location = new Location(

                               null,
                               CountryCodeBody ?? CountryCodeURL!.Value,
                               PartyIdBody     ?? PartyIdURL!.    Value,
                               LocationIdBody  ?? LocationIdURL!. Value,
                               LocationType,
                               Address,
                               City,
                               PostalCode,
                               Country,
                               Coordinates,

                               Name,
                               RelatedLocations,
                               EVSEs,
                               Directions,
                               Operator,
                               Suboperator,
                               Owner,
                               Facilities,
                               TimeZone,
                               OpeningTimes,
                               ChargingWhenClosed,
                               Images,
                               EnergyMix,

                               Publish,

                               null,
                               null,

                               Created,
                               LastUpdated

                           );

                if (CustomLocationParser is not null)
                    Location = CustomLocationParser(JSON,
                                                    Location);

                return true;

            }
            catch (Exception e)
            {
                Location       = default;
                ErrorResponse  = "The given JSON representation of a location is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(IncludeOwnerInformation = false, IncludeEnergyMeter = false, EMSPId = null, CustomLocationSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeOwnerInformation">Include optional owner information.</param>
        /// <param name="IncludeEnergyMeter">Whether to include the energy meter.</param>
        /// <param name="EMSPId">The optional EMSP identification, e.g. for including the right charging tariff.</param>
        /// <param name="CustomLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomAdditionalGeoLocationSerializer">A delegate to serialize custom additional geo location JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomLocationEnergyMeterSerializer">A delegate to serialize custom location energy meter JSON objects.</param>
        /// <param name="CustomEVSEEnergyMeterSerializer">A delegate to serialize custom EVSE energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomHoursSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        public JObject ToJSON(Boolean                                                       IncludeOwnerInformation                      = false,
                              Boolean                                                       IncludeEnergyMeter                           = false,
                              EMSP_Id?                                                      EMSPId                                       = null,
                              CustomJObjectSerializerDelegate<Location>?                    CustomLocationSerializer                     = null,
                              CustomJObjectSerializerDelegate<AdditionalGeoLocation>?       CustomAdditionalGeoLocationSerializer        = null,
                              CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                              CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                              CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                              CustomJObjectSerializerDelegate<EnergyMeter<Location>>?       CustomLocationEnergyMeterSerializer          = null,
                              CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?           CustomEVSEEnergyMeterSerializer              = null,
                              CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                              CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                              CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>?             CustomBusinessDetailsSerializer              = null,
                              CustomJObjectSerializerDelegate<Hours>?                       CustomHoursSerializer                        = null,
                              CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null,
                              CustomJObjectSerializerDelegate<EnergyMix>?                   CustomEnergyMixSerializer                    = null,
                              CustomJObjectSerializerDelegate<EnergySource>?                CustomEnergySourceSerializer                 = null,
                              CustomJObjectSerializerDelegate<EnvironmentalImpact>?         CustomEnvironmentalImpactSerializer          = null)
        {

            var json = JSONObject.Create(

                           IncludeOwnerInformation
                               ? new JProperty("country_code",           CountryCode. ToString())
                               : null,

                           IncludeOwnerInformation
                               ? new JProperty("party_id",               PartyId.     ToString())
                               : null,

                                 new JProperty("id",                     Id.          ToString()),

                                 new JProperty("type",                   LocationType.ToString()),

                           Name.IsNotNullOrEmpty()
                               ? new JProperty("name",                   Name)
                               : null,

                                 new JProperty("address",                Address),
                                 new JProperty("city",                   City),
                                 new JProperty("postal_code",            PostalCode),
                                 new JProperty("country",                Country.Alpha3Code),

                                 new JProperty("coordinates",            new JObject(
                                                                             new JProperty("latitude",  Coordinates.Latitude. Value.ToString("0.00000##").Replace(",", ".")),
                                                                             new JProperty("longitude", Coordinates.Longitude.Value.ToString("0.00000##").Replace(",", "."))
                                                                         )),

                           RelatedLocations.Any()
                               ? new JProperty("related_locations",      new JArray(RelatedLocations.Select (additionalGeoLocation => additionalGeoLocation.ToJSON(CustomAdditionalGeoLocationSerializer,
                                                                                                                                                                   CustomDisplayTextSerializer))))
                               : null,

                           EVSEs.Any()
                               ? new JProperty("evses",                  new JArray(EVSEs.           OrderBy(evse                  => evse.UId).
                                                                                                     Select (evse                  => evse.                 ToJSON(EMSPId,
                                                                                                                                                                   IncludeEnergyMeter,
                                                                                                                                                                   CustomEVSESerializer,
                                                                                                                                                                   CustomStatusScheduleSerializer,
                                                                                                                                                                   CustomConnectorSerializer,
                                                                                                                                                                   CustomEVSEEnergyMeterSerializer,
                                                                                                                                                                   CustomTransparencySoftwareStatusSerializer,
                                                                                                                                                                   CustomTransparencySoftwareSerializer,
                                                                                                                                                                   CustomDisplayTextSerializer,
                                                                                                                                                                   CustomImageSerializer))))
                               : null,

                           Directions.Any()
                               ? new JProperty("directions",             new JArray(Directions.      Select(displayText            => displayText.          ToJSON(CustomDisplayTextSerializer))))
                               : null,

                           Operator is not null
                               ? new JProperty("operator",               Operator.   ToJSON(CustomBusinessDetailsSerializer))
                               : null,

                           SubOperator is not null
                               ? new JProperty("suboperator",            SubOperator.ToJSON(CustomBusinessDetailsSerializer))
                               : null,

                           Owner is not null
                               ? new JProperty("owner",                  Owner.      ToJSON(CustomBusinessDetailsSerializer))
                               : null,

                           Facilities.Any()
                               ? new JProperty("facilities",             new JArray(Facilities.      Select(facility               => facility.             ToString())))
                               : null,

                                 new JProperty("time_zone",              Timezone),

                           OpeningTimes is not null
                               ? new JProperty("opening_times",          OpeningTimes.ToJSON(CustomHoursSerializer))
                               : null,

                           ChargingWhenClosed.HasValue
                               ? new JProperty("charging_when_closed",   ChargingWhenClosed.Value)
                               : null,

                           Images.Any()
                               ? new JProperty("images",                 new JArray(Images.          Select(image                  => image.                ToJSON(CustomImageSerializer))))
                               : null,

                           EnergyMix is not null
                               ? new JProperty("energy_mix",             EnergyMix.  ToJSON(CustomEnergyMixSerializer,
                                                                                            CustomEnergySourceSerializer,
                                                                                            CustomEnvironmentalImpactSerializer))
                               : null,


                           // Non-Standard extensions

                           Publish.HasValue
                               ? new JProperty("publish",                Publish.Value)
                               : null,


                                 new JProperty("created",                Created.    ToISO8601()),
                                 new JProperty("last_updated",           LastUpdated.ToISO8601())

                       );

            return CustomLocationSerializer is not null
                       ? CustomLocationSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this location.
        /// </summary>
        public Location Clone()

            => new (

                   CommonAPI,
                   CountryCode.     Clone(),
                   PartyId.         Clone(),
                   Id.              Clone(),
                   LocationType.    Clone(),
                   Address.         CloneString(),
                   City.            CloneString(),
                   PostalCode.      CloneString(),
                   Country.         Clone(),
                   Coordinates.     Clone(),

                   Name?.           CloneString(),
                   RelatedLocations.Select(relatedLocation => relatedLocation.Clone()),
                   EVSEs.           Select(evse            => evse.           Clone()),
                   Directions.      Select(displayText     => displayText.    Clone()),
                   Operator?.       Clone(),
                   SubOperator?.    Clone(),
                   Owner?.          Clone(),
                   Facilities.      Select(facility        => facility.       Clone()),
                   Timezone?.       CloneString(),
                   OpeningTimes?.   Clone(),
                   ChargingWhenClosed,
                   Images.          Select(image           => image.          Clone()),
                   EnergyMix?.      Clone(),

                   Publish,

                   CustomData,
                   InternalData,

                   Created,
                   LastUpdated

               );

        #endregion


        #region (private) TryPrivatePatch(JSON, Patch)

        private PatchResult<JObject> TryPrivatePatch(JObject           JSON,
                                                     JObject           Patch,
                                                     EventTracking_Id  EventTrackingId)
        {

            foreach (var property in Patch)
            {

                if      (property.Key == "country_code")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'country code' of a location is not allowed!");

                else if (property.Key == "party_id")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'party identification' of a location is not allowed!");

                else if (property.Key == "id")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'identification' of a location is not allowed!");

                else if (property.Key == "evses")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'evses' array of a location is not allowed!");
                //{

                //    if (property.Value == null)
                //        return PatchResult<JObject>.Failed(JSON,
                //                                           "Patching the 'evses' array of a location to 'null' is not allowed!");

                //    else if (property.Value is JArray EVSEArray)
                //    {

                //        if (EVSEArray.Count == 0)
                //            return PatchResult<JObject>.Failed(JSON,
                //                                               "Patching the 'evses' array of a location to '[]' is not allowed!");

                //        else
                //        {
                //            foreach (var evse in EVSEArray)
                //            {

                //                //ToDo: What to do with multiple EVSE objects having the same EVSEUId?
                //                if (evse is JObject EVSEObject)
                //                {

                //                    if (EVSEObject.ParseMandatory("uid",
                //                                                  "internal EVSE identification",
                //                                                  EVSE_UId.TryParse,
                //                                                  out EVSE_UId  EVSEUId,
                //                                                  out String    ErrorResponse))
                //                    {

                //                        return PatchResult<JObject>.Failed(JSON,
                //                                                           "Patching the 'evses' array of a location led to an error: " + ErrorResponse);

                //                    }

                //                    if (TryGetEVSE(EVSEUId, out EVSE EVSE))
                //                    {
                //                        //EVSE.Patch(EVSEObject);
                //                    }
                //                    else
                //                    {

                //                        //ToDo: Create this "new" EVSE!
                //                        return PatchResult<JObject>.Failed(JSON,
                //                                                           "Unknown EVSE UId!");

                //                    }

                //                }
                //                else
                //                {
                //                    return PatchResult<JObject>.Failed(JSON,
                //                                                       "Invalid JSON merge patch for 'evses' array of a location: Data within the 'evses' array is not a valid EVSE object!");
                //                }

                //            }
                //        }
                //    }

                //    else
                //    {
                //        return PatchResult<JObject>.Failed(JSON,
                //                                           "Invalid JSON merge patch for 'evses' array of a location: JSON property 'evses' is not an array!");
                //    }

                //}

                else if (property.Value is null)
                    JSON.Remove(property.Key);

                else if (property.Value is JObject subObject)
                {

                    if (JSON.ContainsKey(property.Key))
                    {

                        if (JSON[property.Key] is JObject oldSubObject)
                        {

                            //ToDo: Perhaps use a more generic JSON patch here!
                            // PatchObject.Apply(ToJSON(), EVSEPatch),
                            var patchResult = TryPrivatePatch(oldSubObject, subObject, EventTrackingId);

                            if (patchResult.IsSuccess)
                                JSON[property.Key] = patchResult.PatchedData;

                        }

                        else
                            JSON[property.Key] = subObject;

                    }

                    else
                        JSON.Add(property.Key, subObject);

                }

                //else if (property.Value is JArray subArray)
                //{
                //}

                else
                    JSON[property.Key] = property.Value;

            }

            return PatchResult<JObject>.Success(EventTrackingId, JSON);

        }

        #endregion

        #region TryPatch(LocationPatch, AllowDowngrades = false)

        /// <summary>
        /// Try to patch the JSON representation of this location.
        /// </summary>
        /// <param name="LocationPatch">The JSON merge patch.</param>
        /// <param name="AllowDowngrades">Allow to set the 'lastUpdated' timestamp to an earlier value.</param>
        public PatchResult<Location> TryPatch(JObject           LocationPatch,
                                              Boolean           AllowDowngrades   = false,
                                              EventTracking_Id? EventTrackingId   = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (!LocationPatch.HasValues)
                return PatchResult<Location>.Failed(
                           EventTrackingId,
                           this,
                           "The given location patch must not be null or empty!"
                       );

            lock (patchLock)
            {

                if (LocationPatch["last_updated"] is null)
                    LocationPatch["last_updated"] = Timestamp.Now.ToISO8601();

                else if (AllowDowngrades == false &&
                        LocationPatch["last_updated"]?.Type == JTokenType.Date &&
                       (LocationPatch["last_updated"]?.Value<DateTime>().ToISO8601().CompareTo(LastUpdated.ToISO8601()) < 1))
                {
                    return PatchResult<Location>.Failed(EventTrackingId, this,
                                                        "The 'lastUpdated' timestamp of the location patch must be newer then the timestamp of the existing location!");
                }


                var patchResult = TryPrivatePatch(ToJSON(), LocationPatch, EventTrackingId);


                if (patchResult.IsFailed)
                    return PatchResult<Location>.Failed(EventTrackingId, this,
                                                        patchResult.ErrorResponse);

                if (TryParse(patchResult.PatchedData,
                             out var patchedLocation,
                             out var errorResponse,
                             CountryCode,
                             PartyId) &&
                    patchedLocation is not null)
                {

                    return PatchResult<Location>.Success(EventTrackingId, patchedLocation,
                                                         errorResponse);

                }

                else
                    return PatchResult<Location>.Failed(EventTrackingId, this,
                                                        "Invalid JSON merge patch of a location: " + errorResponse);

            }

        }

        #endregion


        internal IEnumerable<Tariff_Id> GetTariffIds(EVSE_Id?       EVSEId        = null,
                                                     Connector_Id?  ConnectorId   = null,
                                                     EMSP_Id?       EMSPId        = null)

            => CommonAPI?.GetTariffIds(CountryCode,
                                       PartyId,
                                       Id,
                                       EVSEId,
                                       ConnectorId,
                                       EMSPId) ?? [];


        private readonly ConcurrentDictionary<EVSE_UId, EVSE> evsesByUId  = new();
        private readonly ConcurrentDictionary<EVSE_Id,  EVSE> evsesById   = new();

        #region EVSEExists(EVSEUId)

        /// <summary>
        /// Checks whether any EVSE having the given EVSE identification exists.
        /// </summary>
        /// <param name="EVSEUId">An EVSE identification.</param>
        public Boolean EVSEExists(EVSE_UId EVSEUId)

            => evsesByUId.ContainsKey(EVSEUId);

        #endregion

        #region GetEVSE   (EVSEUId)

        /// <summary>
        /// Return the EVSE having the given EVSE unique identification.
        /// </summary>
        /// <param name="EVSEUId">An EVSE unique identification.</param>
        public EVSE? GetEVSE(EVSE_UId EVSEUId)
        {

            if (evsesByUId.TryGetValue(EVSEUId, out var evse))
                return evse;

            return null;

        }

        #endregion

        #region TryGetEVSE(EVSEUId, out EVSE)

        /// <summary>
        /// Try to return the EVSE having the given EVSE unique identification.
        /// </summary>
        /// <param name="EVSEUId">An EVSE unique identification.</param>
        /// <param name="EVSE">The EVSE having the given EVSE identification.</param>
        public Boolean TryGetEVSE(EVSE_UId                       EVSEUId,
                                  [NotNullWhen(true)] out EVSE?  EVSE)

            => evsesByUId.TryGetValue(EVSEUId, out EVSE);

        #endregion

        #region IEnumerable<EVSE> Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => evsesByUId.Values.GetEnumerator();

        public IEnumerator<EVSE> GetEnumerator()
            => evsesByUId.Values.GetEnumerator();

        #endregion


        #region EVSEExists(EVSEId)

        /// <summary>
        /// Checks whether any EVSE having the given EVSE identification exists.
        /// </summary>
        /// <param name="EVSEId">An EVSE identification.</param>
        public Boolean EVSEExists(EVSE_Id EVSEId)

            => evsesById.ContainsKey(EVSEId);

        #endregion

        #region GetEVSE   (EVSEId)

        /// <summary>
        /// Return the EVSE having the given EVSE identification.
        /// </summary>
        /// <param name="EVSEId">An EVSE identification.</param>
        public EVSE? GetEVSE(EVSE_Id EVSEId)
        {

            if (evsesById.TryGetValue(EVSEId, out var evse))
                return evse!;

            return default;

        }

        #endregion

        #region TryGetEVSE(EVSEId, out EVSE)

        /// <summary>
        /// Try to return the EVSE having the given EVSE identification.
        /// </summary>
        /// <param name="EVSEId">An EVSE identification.</param>
        /// <param name="EVSE">The EVSE having the given EVSE identification.</param>
        public Boolean TryGetEVSE(EVSE_Id    EVSEId,
                                  out EVSE?  EVSE)

            => evsesById.TryGetValue(EVSEId, out EVSE);

        #endregion


        #region Update(LocationBuilder, out Warnings)

        public Location? Update(Action<Builder>           LocationBuilder,
                                out IEnumerable<Warning>  Warnings)
        {

            var builder = ToBuilder();
            LocationBuilder(builder);

            return builder.ToImmutable(out Warnings);

        }

        #endregion


        #region CalcSHA256Hash(CustomLocationSerializer = null, CustomAdditionalGeoLocationSerializer = null, ...)

        /// <summary>
        /// Calculate the SHA256 hash of the JSON representation of this location in HEX.
        /// </summary>
        /// <param name="CustomLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomAdditionalGeoLocationSerializer">A delegate to serialize custom additional geo location JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomLocationEnergyMeterSerializer">A delegate to serialize custom location energy meter JSON objects.</param>
        /// <param name="CustomEVSEEnergyMeterSerializer">A delegate to serialize custom EVSE energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomHoursSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        public String CalcSHA256Hash(EMSP_Id?                                                      EMSPId                                       = null,
                                     CustomJObjectSerializerDelegate<Location>?                    CustomLocationSerializer                     = null,
                                     CustomJObjectSerializerDelegate<AdditionalGeoLocation>?       CustomAdditionalGeoLocationSerializer        = null,
                                     CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                                     CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                                     CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                                     CustomJObjectSerializerDelegate<EnergyMeter<Location>>?       CustomLocationEnergyMeterSerializer          = null,
                                     CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?           CustomEVSEEnergyMeterSerializer              = null,
                                     CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                                     CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                                     CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null,
                                     CustomJObjectSerializerDelegate<BusinessDetails>?             CustomBusinessDetailsSerializer              = null,
                                     CustomJObjectSerializerDelegate<Hours>?                       CustomHoursSerializer                        = null,
                                     CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null,
                                     CustomJObjectSerializerDelegate<EnergyMix>?                   CustomEnergyMixSerializer                    = null)
        {

            ETag = SHA256.HashData(
                       ToJSON(
                           true,
                           true,
                           EMSPId,
                           CustomLocationSerializer,
                           CustomAdditionalGeoLocationSerializer,
                           CustomEVSESerializer,
                           CustomStatusScheduleSerializer,
                           CustomConnectorSerializer,
                           CustomLocationEnergyMeterSerializer,
                           CustomEVSEEnergyMeterSerializer,
                           CustomTransparencySoftwareStatusSerializer,
                           CustomTransparencySoftwareSerializer,
                           CustomDisplayTextSerializer,
                           CustomBusinessDetailsSerializer,
                           CustomHoursSerializer,
                           CustomImageSerializer,
                           CustomEnergyMixSerializer
                       ).ToUTF8Bytes()
                   ).ToBase64();

            return ETag;

        }

        #endregion


        #region Operator overloading

        #region Operator == (Location1, Location2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Location1">A location.</param>
        /// <param name="Location2">Another location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Location? Location1,
                                           Location? Location2)
        {

            if (Object.ReferenceEquals(Location1, Location2))
                return true;

            if (Location1 is null || Location2 is null)
                return false;

            return Location1.Equals(Location2);

        }

        #endregion

        #region Operator != (Location1, Location2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Location1">A location.</param>
        /// <param name="Location2">Another location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Location? Location1,
                                           Location? Location2)

            => !(Location1 == Location2);

        #endregion

        #region Operator <  (Location1, Location2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Location1">A location.</param>
        /// <param name="Location2">Another location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Location? Location1,
                                          Location? Location2)

            => Location1 is null
                   ? throw new ArgumentNullException(nameof(Location1), "The given location must not be null!")
                   : Location1.CompareTo(Location2) < 0;

        #endregion

        #region Operator <= (Location1, Location2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Location1">A location.</param>
        /// <param name="Location2">Another location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Location? Location1,
                                           Location? Location2)

            => !(Location1 > Location2);

        #endregion

        #region Operator >  (Location1, Location2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Location1">A location.</param>
        /// <param name="Location2">Another location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Location? Location1,
                                          Location? Location2)

            => Location1 is null
                   ? throw new ArgumentNullException(nameof(Location1), "The given location must not be null!")
                   : Location1.CompareTo(Location2) > 0;

        #endregion

        #region Operator >= (Location1, Location2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Location1">A location.</param>
        /// <param name="Location2">Another location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Location? Location1,
                                           Location? Location2)

            => !(Location1 < Location2);

        #endregion

        #endregion

        #region IComparable<Location> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two locations.
        /// </summary>
        /// <param name="Object">A location to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Location location
                   ? CompareTo(location)
                   : throw new ArgumentException("The given object is not a charging location!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Location)

        /// <summary>
        /// Compares two locations.
        /// </summary>
        /// <param name="Location">A location to compare with.</param>
        public Int32 CompareTo(Location? Location)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given charging location must not be null!");

            var c = CountryCode. CompareTo(Location.CountryCode);

            if (c == 0)
                c = PartyId.     CompareTo(Location.PartyId);

            if (c == 0)
                c = Id.          CompareTo(Location.Id);

            if (c == 0)
                c = LocationType.CompareTo(Location.LocationType);

            if (c == 0)
                c = Created.     CompareTo(Location.Created);

            if (c == 0)
                c = LastUpdated. CompareTo(Location.LastUpdated);

            if (c == 0)
                c = ETag.        CompareTo(Location.ETag);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Location> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two locations for equality.
        /// </summary>
        /// <param name="Object">A location to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Location location &&
                   Equals(location);

        #endregion

        #region Equals(Location)

        /// <summary>
        /// Compares two locations for equality.
        /// </summary>
        /// <param name="Location">A location to compare with.</param>
        public Boolean Equals(Location? Location)

            => Location is not null &&

               CountryCode.            Equals(Location.CountryCode)             &&
               PartyId.                Equals(Location.PartyId)                 &&
               Id.                     Equals(Location.Id)                      &&
               LocationType.           Equals(Location.LocationType)            &&
               Address.                Equals(Location.Address)                 &&
               City.                   Equals(Location.City)                    &&
               PostalCode.             Equals(Location.PostalCode)              &&
               Country.                Equals(Location.Country)                 &&
               Coordinates.            Equals(Location.Coordinates)             &&
               Created.    ToISO8601().Equals(Location.Created.    ToISO8601()) &&
               LastUpdated.ToISO8601().Equals(Location.LastUpdated.ToISO8601()) &&

             ((Name               is     null &&  Location.Name               is     null) ||
              (Name               is not null &&  Location.Name               is not null && Name.                    Equals(Location.Name)))                     &&

             ((Operator           is     null &&  Location.Operator           is     null) ||
              (Operator           is not null &&  Location.Operator           is not null && Operator.                Equals(Location.Operator)))                 &&

             ((SubOperator        is     null &&  Location.SubOperator        is     null) ||
              (SubOperator        is not null &&  Location.SubOperator        is not null && SubOperator.             Equals(Location.SubOperator)))              &&

             ((Owner              is     null &&  Location.Owner              is     null) ||
              (Owner              is not null &&  Location.Owner              is not null && Owner.                   Equals(Location.Owner)))                    &&

             ((Timezone           is     null &&  Location.Timezone           is     null) ||
              (Timezone           is not null &&  Location.Timezone           is not null && Timezone.                Equals(Location.Timezone)))                 &&

             ((OpeningTimes       is     null &&  Location.OpeningTimes       is     null) ||
              (OpeningTimes       is not null &&  Location.OpeningTimes       is not null && OpeningTimes.            Equals(Location.OpeningTimes)))             &&

            ((!ChargingWhenClosed.HasValue    && !Location.ChargingWhenClosed.HasValue)    ||
              (ChargingWhenClosed.HasValue    &&  Location.ChargingWhenClosed.HasValue    && ChargingWhenClosed.Value.Equals(Location.ChargingWhenClosed.Value))) &&

             ((EnergyMix          is     null &&  Location.EnergyMix          is     null) ||
              (EnergyMix          is not null &&  Location.EnergyMix          is not null && EnergyMix.               Equals(Location.EnergyMix)))                &&

            ((!Publish.           HasValue    && !Location.Publish.           HasValue)    ||
              (Publish.           HasValue    &&  Location.Publish.           HasValue    && Publish.           Value.Equals(Location.Publish.Value)))            &&

               RelatedLocations.Count().Equals(Location.RelatedLocations.Count()) &&
               EVSEs.           Count().Equals(Location.EVSEs.           Count()) &&
               Directions.      Count().Equals(Location.Directions.      Count()) &&
               Facilities.      Count().Equals(Location.Facilities.      Count()) &&
               Images.          Count().Equals(Location.Images.          Count()) &&

               RelatedLocations.All(additionalGeoLocation => Location.RelatedLocations.Contains(additionalGeoLocation)) &&
               EVSEs.           All(evse                  => Location.EVSEs.           Contains(evse))                  &&
               Directions.      All(displayText           => Location.Directions.      Contains(displayText))           &&
               Facilities.      All(facility              => Location.Facilities.      Contains(facility))              &&
               Images.          All(image                 => Location.Images.          Contains(image));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Id,            " (",
                   CountryCode,   "-",
                   PartyId,       "), ",

                   EVSEs.Count(), " EVSE(s), ",

                   LastUpdated.ToISO8601()

               );

        #endregion


        #region ToBuilder(NewLocationId = null)

        /// <summary>
        /// Return a builder for this location.
        /// </summary>
        /// <param name="NewLocationId">An optional new location identification.</param>
        public Builder ToBuilder(Location_Id? NewLocationId = null)

            => new (CommonAPI,
                    CountryCode,
                    PartyId,
                    NewLocationId ?? Id,
                    LocationType,
                    Address,
                    City,
                    PostalCode,
                    Country,
                    Coordinates,

                    Name,
                    RelatedLocations,
                    EVSEs,
                    Directions,
                    Operator,
                    SubOperator,
                    Owner,
                    Facilities,
                    Timezone,
                    OpeningTimes,
                    ChargingWhenClosed,
                    Images,
                    EnergyMix,

                    Publish,

                    CustomData,
                    InternalData,

                    Created,
                    LastUpdated);

        #endregion

        #region (class) Builder

        /// <summary>
        /// A location builder.
        /// </summary>
        public class Builder
        {

            #region Properties

            /// <summary>
            /// The parent CommonAPI of this charging location.
            /// </summary>
            internal CommonAPI?                        CommonAPI                { get; }

            /// <summary>
            /// The ISO-3166 alpha-2 country code of the charge point operator that 'owns' this charging location.
            /// </summary>
            [Mandatory]
            public CountryCode?                        CountryCode              { get; set; }

            /// <summary>
            /// The identification of the charge point operator that 'owns' this charging location (following the ISO-15118 standard).
            /// </summary>
            [Mandatory]
            public Party_Id?                           PartyId                  { get; set; }

            /// <summary>
            /// The identification of the charging location within the CPOs platform (and suboperator platforms).
            /// This field can never be changed, modified or renamed.
            /// </summary>
            [Mandatory]
            public Location_Id?                        Id                       { get; set; }

            /// <summary>
            /// The general type of the charging location.
            /// </summary>
            [Mandatory]
            public LocationType?                       LocationType             { get; set; }

            /// <summary>
            /// The optional display name of the charging location.
            /// string(255)
            /// </summary>
            [Optional]
            public String?                             Name                     { get; set; }

            /// <summary>
            /// The address of the charging location.
            /// string(45)
            /// </summary>
            [Mandatory]
            public String?                             Address                  { get; set; }

            /// <summary>
            /// The city or town of the charging location.
            /// string(45)
            /// </summary>
            [Mandatory]
            public String?                             City                     { get; set; }

            /// <summary>
            /// The postal code of the charging location.
            /// string(10)
            /// </summary>
            [Mandatory]
            public String?                             PostalCode               { get; set; }

            /// <summary>
            /// The country of the charging location.
            /// </summary>
            [Mandatory]
            public Country?                            Country                  { get; set; }

            /// <summary>
            /// The geographical location of this charging location.
            /// </summary>
            [Mandatory]
            public GeoCoordinate?                      Coordinates              { get; set; }

            /// <summary>
            /// The optional enumeration of additional geographical locations of related geo coordinates that might be relevant to the EV driver.
            /// </summary>
            [Optional]
            public HashSet<AdditionalGeoLocation>      RelatedLocations         { get; }

            /// <summary>
            /// The optional enumeration of Electric Vehicle Supply Equipments (EVSE) at this charging location.
            /// </summary>
            [Optional]
            public HashSet<EVSE>                       EVSEs                    { get; }

            /// <summary>
            /// The optional enumeration of all EVSE identifications at this charging location.
            /// </summary>
            [Optional]
            public IEnumerable<EVSE_Id>                EVSEIds
                => EVSEs.Where (evse => evse.EVSEId.HasValue).
                         Select(evse => evse.EVSEId!.Value);

            /// <summary>
            /// The enumeration of all internal EVSE (unique) identifications at this charging location.
            /// </summary>
            [Mandatory]
            public IEnumerable<EVSE_UId>               EVSEUIds
                => EVSEs.Select(evse => evse.UId);

            /// <summary>
            /// The optional enumeration of human-readable directions on how to reach the location.
            /// </summary>
            [Optional]
            public HashSet<DisplayText>                Directions               { get; }

            /// <summary>
            /// Optional information about the charging station operator.
            /// </summary>
            /// <remarks>When not specified, the information retrieved from the credentials module matching the country_code and party_id should be used instead.</remarks>
            [Optional]
            public BusinessDetails?                    Operator                 { get; set; }

            /// <summary>
            /// Optional information about the suboperator.
            /// </summary>
            [Optional]
            public BusinessDetails?                    SubOperator              { get; set; }

            /// <summary>
            /// Optional information about the owner.
            /// </summary>
            [Optional]
            public BusinessDetails?                    Owner                    { get; set; }

            /// <summary>
            /// The optional enumeration of facilities this charging location directly belongs to.
            /// </summary>
            [Optional]
            public HashSet<Facilities>                 Facilities               { get; }

            /// <summary>
            /// One of IANA tzdata’s TZ-values representing the time zone of the charging location (http://www.iana.org/time-zones).
            /// </summary>
            /// <example>"Europe/Oslo", "Europe/Zurich"</example>
            [Mandatory]
            public String?                             Timezone                 { get; set; }

            /// <summary>
            /// The optional times when the EVSEs at the charging location can be accessed for charging.
            /// </summary>
            [Optional]
            public Hours?                              OpeningTimes             { get; set; }

            /// <summary>
            /// Indicates if the EVSEs are still charging outside the opening
            /// hours of the charging location. E.g. when the parking garage closes its
            /// barriers over night, is it allowed to charge till the next
            /// morning? [Default: true]
            /// </summary>
            [Optional]
            public Boolean?                            ChargingWhenClosed       { get; set; }

            /// <summary>
            /// The optional enumeration of images related to the charging location such as photos or logos.
            /// </summary>
            [Optional]
            public HashSet<Image>                      Images                   { get; }

            /// <summary>
            /// Optional details on the energy supplied at this charging location.
            /// </summary>
            [Optional]
            public EnergyMix?                          EnergyMix                { get; set; }



            /// <summary>
            /// Whether this charging location may be published on an website or app etc., or not.
            /// </summary>
            [Mandatory]
            public Boolean?                            Publish                  { get; set; }


            public JObject                             CustomData               { get; }
            public UserDefinedDictionary               InternalData             { get; }


            // <summary>
            /// The timestamp when this charging location was created.
            /// </summary>
            [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
            public DateTime?                           Created                  { get; set; }

            /// <summary>
            /// The timestamp when this charging location was last updated (or created).
            /// </summary>
            [Mandatory]
            public DateTime?                           LastUpdated              { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new charging location builder.
            /// </summary>
            /// <param name="CountryCode">An ISO-3166 alpha-2 country code of the charge point operator that 'owns' this charging location.</param>
            /// <param name="PartyId">An identification of the charge point operator that 'owns' this charging location (following the ISO-15118 standard).</param>
            /// <param name="Id">An identification of the charging location within the CPOs platform (and suboperator platforms).</param>
            /// <param name="LocationType">An optional general type of parking at the charging location.</param>
            /// <param name="Address">The address of the charging location.</param>
            /// <param name="City">The city or town of the charging location.</param>
            /// <param name="Country">The country of the charging location.</param>
            /// <param name="Coordinates">The geographical location of this charging location.</param>
            /// <param name="Timezone">One of IANA tzdata’s TZ-values representing the time zone of the charging location (http://www.iana.org/time-zones).</param>
            /// 
            /// <param name="Name">An optional display name of the charging location.</param>
            /// <param name="PostalCode">An optional postal code of the charging location.</param>
            /// <param name="RelatedLocations">An optional enumeration of additional geographical locations of related geo coordinates that might be relevant to the EV driver.</param>
            /// <param name="EVSEs">An optional enumeration of Electric Vehicle Supply Equipments (EVSE) at this charging location.</param>
            /// <param name="Directions">An optional enumeration of human-readable directions on how to reach the location.</param>
            /// <param name="Operator">Optional information about the charging station operator.</param>
            /// <param name="SubOperator">Optional information about the suboperator.</param>
            /// <param name="Owner">Optional information about the owner.</param>
            /// <param name="Facilities">An optional enumeration of facilities this charging location directly belongs to.</param>
            /// <param name="OpeningTimes">An optional times when the EVSEs at the charging location can be accessed for charging.</param>
            /// <param name="ChargingWhenClosed">Indicates if the EVSEs are still charging outside the opening hours of the charging location. </param>
            /// <param name="Images">An optional enumeration of images related to the charging location such as photos or logos.</param>
            /// <param name="EnergyMix">Optional details on the energy supplied at this charging location.</param>
            /// 
            /// <param name="Publish">Whether this charging location may be published on an website or app etc., or not.</param>
            public Builder(CommonAPI?                           CommonAPI,
                           CountryCode                          CountryCode,
                           Party_Id                             PartyId,
                           Location_Id?                         Id                   = null,
                           LocationType?                        LocationType         = null,
                           String?                              Address              = null,
                           String?                              City                 = null,
                           String?                              PostalCode           = null,
                           Country?                             Country              = null,
                           GeoCoordinate?                       Coordinates          = null,

                           String?                              Name                 = null,
                           IEnumerable<AdditionalGeoLocation>?  RelatedLocations     = null,
                           IEnumerable<EVSE>?                   EVSEs                = null,
                           IEnumerable<DisplayText>?            Directions           = null,
                           BusinessDetails?                     Operator             = null,
                           BusinessDetails?                     SubOperator          = null,
                           BusinessDetails?                     Owner                = null,
                           IEnumerable<Facilities>?             Facilities           = null,
                           String?                              Timezone             = null,
                           Hours?                               OpeningTimes         = null,
                           Boolean?                             ChargingWhenClosed   = null,
                           IEnumerable<Image>?                  Images               = null,
                           EnergyMix?                           EnergyMix            = null,

                           Boolean?                             Publish              = null,

                           JObject?                             CustomData           = null,
                           UserDefinedDictionary?               InternalData         = null,

                           DateTime?                            Created              = null,
                           DateTime?                            LastUpdated          = null)

            {

                this.CommonAPI           = CommonAPI;
                this.CountryCode         = CountryCode;
                this.PartyId             = PartyId;
                this.Id                  = Id;
                this.LocationType        = LocationType;
                this.Address             = Address;
                this.City                = City;
                this.PostalCode          = PostalCode;
                this.Country             = Country;
                this.Coordinates         = Coordinates;

                this.Name                = Name;
                this.RelatedLocations    = RelatedLocations is not null ? [.. RelatedLocations] : [];
                this.EVSEs               = EVSEs            is not null ? [.. EVSEs]            : [];
                this.Directions          = Directions       is not null ? [.. Directions]       : [];
                this.Operator            = Operator;
                this.SubOperator         = SubOperator;
                this.Owner               = Owner;
                this.Facilities          = Facilities       is not null ? [.. Facilities]       : [];
                this.Timezone            = Timezone;
                this.OpeningTimes        = OpeningTimes;
                this.ChargingWhenClosed  = ChargingWhenClosed;
                this.Images              = Images           is not null ? [.. Images]           : [];
                this.EnergyMix           = EnergyMix;

                this.Publish             = Publish;

                this.CustomData          = CustomData   ?? [];
                this.InternalData        = InternalData ?? new UserDefinedDictionary();

                this.Created             = Created;
                this.LastUpdated         = LastUpdated;

            }

            #endregion


            #region SetEVSE   (EVSE)

            public Builder SetEVSE(EVSE EVSE)
            {

                var newEVSEs = EVSEs.Where(evse => evse.UId != EVSE.UId).ToHashSet();
                EVSEs.Clear();

                foreach (var newEVSE in newEVSEs)
                    EVSEs.Add(newEVSE);

                EVSEs.Add(EVSE);

                return this;

            }

            #endregion

            #region RemoveEVSE(EVSE)

            public Builder RemoveEVSE(EVSE EVSE)
            {

                var newEVSEs = EVSEs.Where(evse => evse.UId != EVSE.UId).ToHashSet();
                EVSEs.Clear();

                foreach (var newEVSE in newEVSEs)
                    EVSEs.Add(newEVSE);

                return this;

            }

            #endregion


            #region ToImmutable(out Warnings)

            /// <summary>
            /// Return an immutable version of the location.
            /// </summary>
            public static implicit operator Location?(Builder Builder)

                => Builder.ToImmutable(out _);


            /// <summary>
            /// Return an immutable version of the location.
            /// </summary>
            /// <param name="Warnings"></param>
            public Location? ToImmutable(out IEnumerable<Warning> Warnings)
            {

                var warnings = new List<Warning>();

                if (!CountryCode. HasValue)
                    warnings.Add(Warning.Create("The country code must not be null or empty!"));

                if (!PartyId.     HasValue)
                    warnings.Add(Warning.Create("The party identification must not be null or empty!"));

                if (!Id.          HasValue)
                    warnings.Add(Warning.Create("The location identification must not be null or empty!"));

                if (!LocationType.HasValue)
                    warnings.Add(Warning.Create("The location type must not be null or empty!"));

                if (Address    is null || Address.   IsNullOrEmpty())
                    warnings.Add(Warning.Create("The address parameter must not be null or empty!"));

                if (City       is null || City.      IsNullOrEmpty())
                    warnings.Add(Warning.Create("The city parameter must not be null or empty!"));

                if (PostalCode is null || PostalCode.IsNullOrEmpty())
                    warnings.Add(Warning.Create("The postal code must not be null or empty!"));

                if (Country is null)
                    warnings.Add(Warning.Create("The country parameter must not be null or empty!"));

                if (!Coordinates.HasValue)
                    warnings.Add(Warning.Create("The geo coordinates must not be null or empty!"));

                Warnings = warnings;


                return warnings.Count > 0
                           ? null
                           : new Location(

                                 CommonAPI,
                                 CountryCode!. Value,
                                 PartyId!.     Value,
                                 Id!.          Value,
                                 LocationType!.Value,
                                 Address!,
                                 City!,
                                 PostalCode!,
                                 Country!,
                                 Coordinates!.Value,

                                 Name,
                                 RelatedLocations,
                                 EVSEs,
                                 Directions,
                                 Operator,
                                 SubOperator,
                                 Owner,
                                 Facilities,
                                 Timezone,
                                 OpeningTimes,
                                 ChargingWhenClosed,
                                 Images,
                                 EnergyMix,

                                 Publish,

                                 CustomData,
                                 InternalData,

                                 Created     ?? Timestamp.Now,
                                 LastUpdated ?? Timestamp.Now

                             );

            }

            #endregion

        }

        #endregion


    }

}
