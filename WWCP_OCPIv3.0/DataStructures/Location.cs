/*
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv3_0.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    public delegate IEnumerable<Tariff_Id>  GetTariffIds2_Delegate(CountryCode    CPOCountryCode,
                                                                   Party_Idv3     CPOPartyId,
                                                                   Location_Id?   Location      = null,
                                                                   EVSE_UId?      EVSEUId       = null,
                                                                   Connector_Id?  ConnectorId   = null,
                                                                   EMSP_Id?       EMSPId        = null);


    /// <summary>
    /// The location is a group of EVSEs at more or less the same geographical location
    /// and operated by the same charge point operator.
    /// 
    /// Typically a location is the exact location of the group
    /// of EVSEs, but it can also be the entrance of a parking garage
    /// which contains these EVSEs. The exact way to reach each EVSE
    /// can then be further specified by its own properties.
    /// </summary>
    public class Location : APartyIssuedObject<Location_Id>,
                            IEquatable<Location>,
                            IComparable<Location>,
                            IComparable,
                            IEnumerable<ChargingStation>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of locations.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/location");

        private readonly ConcurrentDictionary<ChargingStation_Id, ChargingStation>        chargingPool = [];

        private readonly ConcurrentDictionary<EnergyMeter_Id,     EnergyMeter<Location>>  energyMeters = [];

        #endregion

        #region Properties

        /// <summary>
        /// Whether the receiving Party or Platform may publish the Location.
        /// When this is set to false, the receiving Party or Platform MAY NOT disclose information from this Location object
        /// to anyone not holding a Token listed in the field publish_allowed_to.
        /// When the same physical facility has both some EVSEs that may be published and other ones that may not be published,
        /// the sender Party SHOULD send two separate Location objects for the two groups of EVSEs.
        /// </summary>
        [Mandatory]
        public Boolean                             Publish                  { get; }

        /// <summary>
        /// One of IANA tzdata’s TZ-values representing the time zone of the location (http://www.iana.org/time-zones).
        /// </summary>
        /// <example>"Europe/Oslo", "Europe/Zurich"</example>
        [Mandatory]
        public String                              Timezone                 { get; }

        /// <summary>
        /// This field SHALL NOT be used unless the publish field is set to false.
        /// Only holders of Tokens that match all the set fields of one PublishToken in the list are allowed to be shown this Location.
        /// </summary>
        [Optional]
        public IEnumerable<PublishToken>           PublishAllowedTo         { get; }

        /// <summary>
        /// Display name of the Location.
        /// </summary>
        [Optional]
        public String?                             Name                     { get; }

        /// <summary>
        /// Address and geographical location of the Location.
        /// This has to be present unless the publish field is set to false.
        /// </summary>
        [Optional]
        public Address?                            Address                  { get; }

        /// <summary>
        /// The optional enumeration of geographical locations of related points relevant to the user.
        /// </summary>
        [Optional]
        public IEnumerable<AdditionalGeoLocation>  RelatedLocations         { get; }

        /// <summary>
        /// The optional general type of parking at the location.
        /// </summary>
        [Optional]
        public ParkingType?                        ParkingType              { get; }

        /// <summary>
        /// The Charging Pool of this Location, that is, the list of Charging Stations
        /// that make up the physical charging infrastructure of this Location.
        /// </summary>
        [Mandatory]
        public IEnumerable<ChargingStation>        ChargingPool
            => chargingPool.Values;

        /// <summary>
        /// The optional enumeration of human-readable directions on how to reach the location.
        /// </summary>
        [Optional]
        public IEnumerable<DisplayText>            Directions               { get; }

        /// <summary>
        /// Information of the operator. When not specified, the information retrieved with Use Case Request Parties Served by Platform,
        /// selected by the Party ID of the Party that issued this Location, MAY be used instead.
        /// </summary>
        [Optional]
        public BusinessDetails?                    Operator                 { get; }

        /// <summary>
        /// Optional information about the suboperator.
        /// </summary>
        [Optional]
        public BusinessDetails?                    SubOperator              { get; }

        /// <summary>
        /// Optional information about the owner.
        /// </summary>
        [Optional]
        public BusinessDetails?                    Owner                    { get; }

        /// <summary>
        /// The optional enumeration of services that are offered at the Location by the CPO or their affiliated partners.
        /// </summary>
        [Optional]
        public IEnumerable<LocationService>        Services                 { get; }

        /// <summary>
        /// The optional enumeration of facilities this location directly belongs to.
        /// </summary>
        [Optional]
        public IEnumerable<Facilities>             Facilities               { get; }

        /// <summary>
        /// The optional times when the EVSEs at the location can be accessed for charging.
        /// </summary>
        [Optional]
        public Hours?                              OpeningTimes             { get; }

        /// <summary>
        /// Indicates if the EVSEs are still charging outside the opening
        /// hours of the location. E.g. when the parking garage closes its
        /// barriers over night, is it allowed to charge till the next
        /// morning? [Default: true]
        /// </summary>
        [Optional]
        public Boolean?                            ChargingWhenClosed       { get; }

        /// <summary>
        /// The optional enumeration of images related to the location such as photos or logos.
        /// </summary>
        [Optional]
        public IEnumerable<Image>                  Images                   { get; }

        /// <summary>
        /// Optional details on the energy supplied at this location.
        /// </summary>
        [Optional]
        public EnergyMix?                          EnergyMix                { get; }

        /// <summary>
        /// How much power or current this Location can draw from the grid at any one time.
        /// </summary>
        [Optional]
        public LocationMaxPower?                   MaxPower                 { get; }

        /// <summary>
        /// A telephone number that a Driver using the Location may call for assistance.Calling this number will typically
        /// connect the caller to the CPO’s customer service department.
        /// </summary>
        [Optional]
        public PhoneNumber?                        HelpPhone                { get; }

        /// <summary>
        /// The optional enumeration of energy meters, e.g. at the grid connection point.
        /// </summary>
        [Optional]
        public IEnumerable<EnergyMeter<Location>>  EnergyMeters
            => energyMeters.Values;


        /// <summary>
        /// The timestamp when this location was created.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
        public DateTime                            Created                  { get; }

        /// <summary>
        /// The timestamp when this location was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                            LastUpdated              { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this location.
        /// </summary>
        public String                              ETag                     { get; private set; }

        #endregion

        #region Constructor(s)

        #region Location(...)

        /// <summary>
        /// Create a new location.
        /// </summary>
        /// <param name="PartyId">The party identification of the party that issued this location.</param>
        /// <param name="Id">An identification of the location within the party.</param>
        /// <param name="VersionId">The version identification of the location.</param>
        /// 
        /// <param name="Publish">Whether this location may be published on an website or app etc., or not.</param>
        /// <param name="Timezone">One of IANA tzdata’s TZ-values representing the time zone of the location (http://www.iana.org/time-zones).</param>
        /// 
        /// <param name="PublishAllowedTo">An optional enumeration of publish tokens. Only owners of tokens that match all the set fields of one publish token in the list are allowed to be shown this location.</param>
        /// <param name="Name">An optional display name of the location.</param>
        /// <param name="Address">The address of the location.</param>
        /// <param name="RelatedLocations">An optional enumeration of additional geographical locations of related geo coordinates that might be relevant to the EV driver.</param>
        /// <param name="ParkingType">An optional general type of parking at the location.</param>
        /// <param name="ChargingPool">An optional enumeration of charging stations at this location.</param>
        /// <param name="Directions">An optional enumeration of human-readable directions on how to reach the location.</param>
        /// <param name="Operator">Optional information about the charging station operator.</param>
        /// <param name="SubOperator">Optional information about the suboperator.</param>
        /// <param name="Owner">Optional information about the owner.</param>
        /// <param name="Services">An optional enumeration of services that are offered at the Location by the CPO or their affiliated partners.</param>
        /// <param name="Facilities">An optional enumeration of facilities this location directly belongs to.</param>
        /// <param name="OpeningTimes">An optional times when the EVSEs at the location can be accessed for charging.</param>
        /// <param name="ChargingWhenClosed">Indicates if the EVSEs are still charging outside the opening hours of the location. </param>
        /// <param name="Images">An optional enumeration of images related to the location such as photos or logos.</param>
        /// <param name="EnergyMix">Optional details on the energy supplied at this location.</param>
        /// <param name="EnergyMeters">An optional enumeration of energy meters, e.g. at the grid connection point.</param>
        /// <param name="MaxPower">How much power or current this Location can draw from the grid at any one time.</param>
        /// <param name="HelpPhone">A telephone number that a Driver using the Location may call for assistance. Calling this number will typically connect the caller to the CPO’s customer service department.</param>
        /// 
        /// <param name="Created">An optional timestamp when this location was created.</param>
        /// <param name="LastUpdated">An optional timestamp when this location was last updated (or created).</param>
        /// 
        /// <param name="CustomLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomPublishTokenSerializer">A delegate to serialize custom publish token type JSON objects.</param>
        /// <param name="CustomAdditionalGeoLocationSerializer">A delegate to serialize custom additional geo location JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomLocationEnergyMeterSerializer">A delegate to serialize custom location energy meter JSON objects.</param>
        /// <param name="CustomChargingStationEnergyMeterSerializer">A delegate to serialize custom charging station energy meter JSON objects.</param>
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
        public Location(Party_Idv3                                                      PartyId,
                        Location_Id                                                     Id,
                        UInt64                                                          VersionId,

                        Boolean                                                         Publish,
                        String                                                          Timezone,

                        IEnumerable<PublishToken>?                                      PublishAllowedTo                             = null,
                        String?                                                         Name                                         = null,
                        Address?                                                        Address                                      = null,
                        IEnumerable<AdditionalGeoLocation>?                             RelatedLocations                             = null,
                        ParkingType?                                                    ParkingType                                  = null,
                        IEnumerable<ChargingStation>?                                   ChargingPool                                 = null,
                        IEnumerable<DisplayText>?                                       Directions                                   = null,
                        BusinessDetails?                                                Operator                                     = null,
                        BusinessDetails?                                                SubOperator                                  = null,
                        BusinessDetails?                                                Owner                                        = null,
                        IEnumerable<LocationService>?                                   Services                                     = null,
                        IEnumerable<Facilities>?                                        Facilities                                   = null,
                        Hours?                                                          OpeningTimes                                 = null,
                        Boolean?                                                        ChargingWhenClosed                           = null,
                        IEnumerable<Image>?                                             Images                                       = null,
                        EnergyMix?                                                      EnergyMix                                    = null,
                        IEnumerable<EnergyMeter<Location>>?                             EnergyMeters                                 = null,
                        LocationMaxPower?                                               MaxPower                                     = null,
                        PhoneNumber?                                                    HelpPhone                                    = null,

                        DateTime?                                                       Created                                      = null,
                        DateTime?                                                       LastUpdated                                  = null,

                        CustomJObjectSerializerDelegate<Location>?                      CustomLocationSerializer                     = null,
                        CustomJObjectSerializerDelegate<PublishToken>?                  CustomPublishTokenSerializer                 = null,
                        CustomJObjectSerializerDelegate<Address>?                       CustomAddressSerializer                      = null,
                        CustomJObjectSerializerDelegate<AdditionalGeoLocation>?         CustomAdditionalGeoLocationSerializer        = null,
                        CustomJObjectSerializerDelegate<ChargingStation>?               CustomChargingStationSerializer              = null,
                        CustomJObjectSerializerDelegate<EVSE>?                          CustomEVSESerializer                         = null,
                        CustomJObjectSerializerDelegate<Connector>?                     CustomConnectorSerializer                    = null,
                        CustomJObjectSerializerDelegate<Parking>?                       CustomParkingSerializer                      = null,
                        CustomJObjectSerializerDelegate<ParkingRestriction>?            CustomParkingRestrictionSerializer           = null,
                        CustomJObjectSerializerDelegate<Image>?                         CustomImageSerializer                        = null,
                        CustomJObjectSerializerDelegate<StatusSchedule>?                CustomStatusScheduleSerializer               = null,
                        CustomJObjectSerializerDelegate<EnergyMeter<Location>>?         CustomLocationEnergyMeterSerializer          = null,
                        CustomJObjectSerializerDelegate<EnergyMeter<ChargingStation>>?  CustomChargingStationEnergyMeterSerializer   = null,
                        CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?             CustomEVSEEnergyMeterSerializer              = null,
                        CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?    CustomTransparencySoftwareStatusSerializer   = null,
                        CustomJObjectSerializerDelegate<TransparencySoftware>?          CustomTransparencySoftwareSerializer         = null,
                        CustomJObjectSerializerDelegate<DisplayText>?                   CustomDisplayTextSerializer                  = null,
                        CustomJObjectSerializerDelegate<BusinessDetails>?               CustomBusinessDetailsSerializer              = null,
                        CustomJObjectSerializerDelegate<Hours>?                         CustomHoursSerializer                        = null,
                        CustomJObjectSerializerDelegate<EnergyMix>?                     CustomEnergyMixSerializer                    = null,
                        CustomJObjectSerializerDelegate<EnergySource>?                  CustomEnergySourceSerializer                 = null,
                        CustomJObjectSerializerDelegate<EnvironmentalImpact>?           CustomEnvironmentalImpactSerializer          = null)

            : this(null,
                   PartyId,
                   Id,
                   VersionId,

                   Publish,
                   Timezone,

                   PublishAllowedTo,
                   Name,
                   Address,
                   RelatedLocations,
                   ParkingType,
                   ChargingPool,
                   Directions,
                   Operator,
                   SubOperator,
                   Owner,
                   Services,
                   Facilities,
                   OpeningTimes,
                   ChargingWhenClosed,
                   Images,
                   EnergyMix,
                   EnergyMeters,
                   MaxPower,
                   HelpPhone,

                   Created     ?? LastUpdated,
                   LastUpdated ?? Created,

                   CustomLocationSerializer,
                   CustomPublishTokenSerializer,
                   CustomAddressSerializer,
                   CustomAdditionalGeoLocationSerializer,
                   CustomChargingStationSerializer,
                   CustomEVSESerializer,
                   CustomConnectorSerializer,
                   CustomParkingSerializer,
                   CustomParkingRestrictionSerializer,
                   CustomImageSerializer,
                   CustomStatusScheduleSerializer,
                   CustomLocationEnergyMeterSerializer,
                   CustomChargingStationEnergyMeterSerializer,
                   CustomEVSEEnergyMeterSerializer,
                   CustomTransparencySoftwareStatusSerializer,
                   CustomTransparencySoftwareSerializer,
                   CustomDisplayTextSerializer,
                   CustomBusinessDetailsSerializer,
                   CustomHoursSerializer,
                   CustomEnergyMixSerializer,
                   CustomEnergySourceSerializer,
                   CustomEnvironmentalImpactSerializer)

        { }

        #endregion

        #region (internal) Location(CommonAPI, ...)

        /// <summary>
        /// Create a new location.
        /// </summary>
        /// <param name="CommonAPI">The OCPI Common API hosting this location.</param>
        /// <param name="PartyId">The party identification of the party that issued this location.</param>
        /// <param name="Id">An identification of the location within the party.</param>
        /// <param name="VersionId">The version identification of the location.</param>
        /// 
        /// <param name="Publish">Whether this location may be published on an website or app etc., or not.</param>
        /// <param name="Timezone">One of IANA tzdata’s TZ-values representing the time zone of the location (http://www.iana.org/time-zones).</param>
        /// 
        /// <param name="PublishAllowedTo">An optional enumeration of publish tokens. Only owners of tokens that match all the set fields of one publish token in the list are allowed to be shown this location.</param>
        /// <param name="Name">An optional display name of the location.</param>
        /// <param name="Address">The address of the location.</param>
        /// <param name="RelatedLocations">An optional enumeration of additional geographical locations of related geo coordinates that might be relevant to the EV driver.</param>
        /// <param name="ParkingType">An optional general type of parking at the location.</param>
        /// <param name="ChargingPool">An optional enumeration of charging stations at this location.</param>
        /// <param name="Directions">An optional enumeration of human-readable directions on how to reach the location.</param>
        /// <param name="Operator">Optional information about the charging station operator.</param>
        /// <param name="SubOperator">Optional information about the suboperator.</param>
        /// <param name="Owner">Optional information about the owner.</param>
        /// <param name="Services">An optional enumeration of services that are offered at the Location by the CPO or their affiliated partners.</param>
        /// <param name="Facilities">An optional enumeration of facilities this location directly belongs to.</param>
        /// <param name="OpeningTimes">An optional times when the EVSEs at the location can be accessed for charging.</param>
        /// <param name="ChargingWhenClosed">Indicates if the EVSEs are still charging outside the opening hours of the location. </param>
        /// <param name="Images">An optional enumeration of images related to the location such as photos or logos.</param>
        /// <param name="EnergyMix">Optional details on the energy supplied at this location.</param>
        /// <param name="EnergyMeters">An optional enumeration of energy meters, e.g. at the grid connection point.</param>
        /// <param name="MaxPower">How much power or current this Location can draw from the grid at any one time.</param>
        /// <param name="HelpPhone">A telephone number that a Driver using the Location may call for assistance. Calling this number will typically connect the caller to the CPO’s customer service department.</param>
        /// 
        /// <param name="Created">An optional timestamp when this location was created.</param>
        /// <param name="LastUpdated">An optional timestamp when this location was last updated (or created).</param>
        /// 
        /// <param name="CustomLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomPublishTokenSerializer">A delegate to serialize custom publish token type JSON objects.</param>
        /// <param name="CustomAdditionalGeoLocationSerializer">A delegate to serialize custom additional geo location JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomLocationEnergyMeterSerializer">A delegate to serialize custom location energy meter JSON objects.</param>
        /// <param name="CustomChargingStationEnergyMeterSerializer">A delegate to serialize custom charging station energy meter JSON objects.</param>
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
        internal Location(CommonAPI?                                                      CommonAPI,
                          Party_Idv3                                                      PartyId,
                          Location_Id                                                     Id,
                          UInt64                                                          VersionId,

                          Boolean                                                         Publish,
                          String                                                          Timezone,

                          IEnumerable<PublishToken>?                                      PublishAllowedTo                             = null,
                          String?                                                         Name                                         = null,
                          Address?                                                        Address                                      = null,
                          IEnumerable<AdditionalGeoLocation>?                             RelatedLocations                             = null,
                          ParkingType?                                                    ParkingType                                  = null,
                          IEnumerable<ChargingStation>?                                   ChargingPool                                 = null,
                          IEnumerable<DisplayText>?                                       Directions                                   = null,
                          BusinessDetails?                                                Operator                                     = null,
                          BusinessDetails?                                                SubOperator                                  = null,
                          BusinessDetails?                                                Owner                                        = null,
                          IEnumerable<LocationService>?                                   Services                                     = null,
                          IEnumerable<Facilities>?                                        Facilities                                   = null,
                          Hours?                                                          OpeningTimes                                 = null,
                          Boolean?                                                        ChargingWhenClosed                           = null,
                          IEnumerable<Image>?                                             Images                                       = null,
                          EnergyMix?                                                      EnergyMix                                    = null,
                          IEnumerable<EnergyMeter<Location>>?                             EnergyMeters                                 = null,
                          LocationMaxPower?                                               MaxPower                                     = null,
                          PhoneNumber?                                                    HelpPhone                                    = null,

                          DateTime?                                                       Created                                      = null,
                          DateTime?                                                       LastUpdated                                  = null,

                          CustomJObjectSerializerDelegate<Location>?                      CustomLocationSerializer                     = null,
                          CustomJObjectSerializerDelegate<PublishToken>?                  CustomPublishTokenSerializer                 = null,
                          CustomJObjectSerializerDelegate<Address>?                       CustomAddressSerializer                      = null,
                          CustomJObjectSerializerDelegate<AdditionalGeoLocation>?         CustomAdditionalGeoLocationSerializer        = null,
                          CustomJObjectSerializerDelegate<ChargingStation>?               CustomChargingStationSerializer              = null,
                          CustomJObjectSerializerDelegate<EVSE>?                          CustomEVSESerializer                         = null,
                          CustomJObjectSerializerDelegate<Connector>?                     CustomConnectorSerializer                    = null,
                          CustomJObjectSerializerDelegate<Parking>?                       CustomParkingSerializer                      = null,
                          CustomJObjectSerializerDelegate<ParkingRestriction>?            CustomParkingRestrictionSerializer           = null,
                          CustomJObjectSerializerDelegate<Image>?                         CustomImageSerializer                        = null,
                          CustomJObjectSerializerDelegate<StatusSchedule>?                CustomStatusScheduleSerializer               = null,
                          CustomJObjectSerializerDelegate<EnergyMeter<Location>>?         CustomLocationEnergyMeterSerializer          = null,
                          CustomJObjectSerializerDelegate<EnergyMeter<ChargingStation>>?  CustomChargingStationEnergyMeterSerializer   = null,
                          CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?             CustomEVSEEnergyMeterSerializer              = null,
                          CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?    CustomTransparencySoftwareStatusSerializer   = null,
                          CustomJObjectSerializerDelegate<TransparencySoftware>?          CustomTransparencySoftwareSerializer         = null,
                          CustomJObjectSerializerDelegate<DisplayText>?                   CustomDisplayTextSerializer                  = null,
                          CustomJObjectSerializerDelegate<BusinessDetails>?               CustomBusinessDetailsSerializer              = null,
                          CustomJObjectSerializerDelegate<Hours>?                         CustomHoursSerializer                        = null,
                          CustomJObjectSerializerDelegate<EnergyMix>?                     CustomEnergyMixSerializer                    = null,
                          CustomJObjectSerializerDelegate<EnergySource>?                  CustomEnergySourceSerializer                 = null,
                          CustomJObjectSerializerDelegate<EnvironmentalImpact>?           CustomEnvironmentalImpactSerializer          = null)

            : base(CommonAPI,
                   PartyId,
                   Id,
                   VersionId)

        {

            this.Publish             = Publish;
            this.Timezone            = Timezone;

            this.PublishAllowedTo    = PublishAllowedTo?.Distinct() ?? [];
            this.Name                = Name;
            this.Address             = Address;
            this.RelatedLocations    = RelatedLocations?.Distinct() ?? [];
            this.ParkingType         = ParkingType;
            this.Directions          = Directions?.      Distinct() ?? [];
            this.Operator            = Operator;
            this.SubOperator         = SubOperator;
            this.Owner               = Owner;
            this.Services            = Services?.        Distinct() ?? [];
            this.Facilities          = Facilities?.      Distinct() ?? [];
            this.OpeningTimes        = OpeningTimes;
            this.ChargingWhenClosed  = ChargingWhenClosed;
            this.Images              = Images?.          Distinct() ?? [];
            this.EnergyMix           = EnergyMix;
            this.MaxPower            = MaxPower;
            this.HelpPhone           = HelpPhone;

            this.Created             = Created                      ?? LastUpdated ?? Timestamp.Now;
            this.LastUpdated         = LastUpdated                  ?? Created     ?? Timestamp.Now;

            foreach (var chargingStation in ChargingPool?.Distinct() ?? [])
            {

                chargingStation.ParentLocation = this;

                chargingPool.TryAdd(
                    chargingStation.Id,
                    chargingStation
                );

            }

            foreach (var energyMeter in EnergyMeters?.Distinct() ?? [])
            {

                energyMeter.Parent = this;

                energyMeters.TryAdd(
                    energyMeter.Id,
                    energyMeter
                );

            }

            this.ETag                = CalcSHA256Hash(
                                           CustomLocationSerializer,
                                           CustomPublishTokenSerializer,
                                           CustomAddressSerializer,
                                           CustomAdditionalGeoLocationSerializer,
                                           CustomChargingStationSerializer,
                                           CustomEVSESerializer,
                                           CustomConnectorSerializer,
                                           CustomParkingSerializer,
                                           CustomParkingRestrictionSerializer,
                                           CustomImageSerializer,
                                           CustomStatusScheduleSerializer,
                                           CustomLocationEnergyMeterSerializer,
                                           CustomChargingStationEnergyMeterSerializer,
                                           CustomEVSEEnergyMeterSerializer,
                                           CustomTransparencySoftwareStatusSerializer,
                                           CustomTransparencySoftwareSerializer,
                                           CustomDisplayTextSerializer,
                                           CustomBusinessDetailsSerializer,
                                           CustomHoursSerializer,
                                           CustomEnergyMixSerializer,
                                           CustomEnergySourceSerializer,
                                           CustomEnvironmentalImpactSerializer
                                       );

            unchecked
            {

                hashCode = this.PartyId.            GetHashCode()        * 101 ^
                           this.Id.                 GetHashCode()        *  97 ^
                           this.VersionId.          GetHashCode()        *  89 ^

                           this.Publish.            GetHashCode()        *  83 ^
                           this.Timezone.           GetHashCode()        *  79 ^

                           this.PublishAllowedTo.   CalcHashCode()       *  73 ^
                          (this.Name?.              GetHashCode()  ?? 0) *  71 ^
                          (this.Address?.           GetHashCode()  ?? 0) *  61 ^
                           this.RelatedLocations.   CalcHashCode()       *  59 ^
                          (this.ParkingType?.       GetHashCode()  ?? 0) *  53^
                          (this.ChargingPool?.      CalcHashCode() ?? 0) *  47 ^
                           this.Directions.         CalcHashCode()       *  43 ^
                          (this.Operator?.          GetHashCode()  ?? 0) *  41 ^
                          (this.SubOperator?.       GetHashCode()  ?? 0) *  37 ^
                          (this.Owner?.             GetHashCode()  ?? 0) *  31 ^
                           this.Services.           CalcHashCode()       *  29 ^
                           this.Facilities.         CalcHashCode()       *  23 ^
                          (this.OpeningTimes?.      GetHashCode()  ?? 0) *  19 ^
                          (this.ChargingWhenClosed?.GetHashCode()  ?? 0) *  17 ^
                           this.Images.             CalcHashCode()       *  13 ^
                          (this.EnergyMix?.         GetHashCode()  ?? 0) *  11 ^
                          (this.MaxPower?.          GetHashCode()  ?? 0) *   7 ^
                          (this.HelpPhone?.         GetHashCode()  ?? 0) *   5 ^

                           this.Created.            GetHashCode()        *   3 ^
                           this.LastUpdated.        GetHashCode();

            }

        }

        #endregion

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of a location.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="LocationIdURL">An optional location identification, e.g. from the HTTP URL.</param>
        /// <param name="VersionIdURL">An optional version identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomLocationParser">A delegate to parse custom location JSON objects.</param>
        public static Location Parse(JObject                                 JSON,
                                     Party_Idv3?                             PartyIdURL             = null,
                                     Location_Id?                            LocationIdURL          = null,
                                     UInt64?                                 VersionIdURL           = null,
                                     CustomJObjectParserDelegate<Location>?  CustomLocationParser   = null)
        {

            if (TryParse(JSON,
                         out var location,
                         out var errorResponse,
                         PartyIdURL,
                         LocationIdURL,
                         VersionIdURL,
                         CustomLocationParser))
            {
                return location;
            }

            throw new ArgumentException("The given JSON representation of a location is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Location, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a location.
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
                        null,
                        null,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a location.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Location">The parsed location.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="LocationIdURL">An optional location identification, e.g. from the HTTP URL.</param>
        /// <param name="VersionIdURL">An optional version identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomLocationParser">A delegate to parse custom location JSON objects.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out Location?      Location,
                                       [NotNullWhen(false)] out String?        ErrorResponse,
                                       Party_Idv3?                             PartyIdURL             = null,
                                       Location_Id?                            LocationIdURL          = null,
                                       UInt64?                                 VersionIdURL           = null,
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

                #region Parse PartyId               [optional]

                if (JSON.ParseOptional("party_id",
                                       "party identification",
                                       Party_Idv3.TryParse,
                                       out Party_Idv3? PartyIdBody,
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

                #region Parse VersionId             [optional]

                if (JSON.ParseOptional("version",
                                       "version identification",
                                       out UInt64? VersionIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!VersionIdURL.HasValue && !VersionIdBody.HasValue)
                {
                    ErrorResponse = "The version identification is missing!";
                    return false;
                }

                if (VersionIdURL.HasValue && VersionIdBody.HasValue && VersionIdURL.Value != VersionIdBody.Value)
                {
                    ErrorResponse = "The optional version identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion


                #region Parse Publish               [mandatory]

                if (!JSON.ParseMandatory("publish",
                                         "publish",
                                         out Boolean Publish,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TimeZone              [mandatory]

                if (!JSON.ParseMandatoryText("time_zone",
                                             "time zone",
                                             out String? TimeZone,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse PublishAllowedTo      [optional]

                if (JSON.ParseOptionalHashSetNull("publish_allowed_to",
                                                  "publish allowed to",
                                                  PublishToken.TryParse,
                                                  out HashSet<PublishToken> PublishAllowedTo,
                                                  out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Name                  [optional]

                var Name = JSON.GetString("name");

                #endregion

                #region Parse Address               [mandatory]

                if (!JSON.ParseOptionalJSON("address",
                                            "address",
                                            OCPIv3_0.Address.TryParse,
                                            out Address? Address,
                                            out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

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

                #region Parse ParkingType           [optional]

                if (JSON.ParseOptional("parking_type",
                                       "parking type",
                                       OCPIv3_0.ParkingType.TryParse,
                                       out ParkingType? ParkingType,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ChargingPool          [optional]

                if (JSON.ParseOptionalHashSet("charging_pool",
                                              "charging pool (aka. stations)",
                                              ChargingStation.TryParse,
                                              out HashSet<ChargingStation> ChargingPool,
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
                                           out BusinessDetails? Operator,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse SubOperator           [optional]

                if (JSON.ParseOptionalJSON("suboperator",
                                           "suboperator",
                                           BusinessDetails.TryParse,
                                           out BusinessDetails? SubOperator,
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
                                           out BusinessDetails? Owner,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Services              [optional]

                if (JSON.ParseOptionalHashSet("services",
                                              "location services",
                                              LocationService.TryParse,
                                              out HashSet<LocationService> Services,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Facilities            [optional]

                if (JSON.ParseOptionalHashSet("facilities",
                                              "facilities",
                                              OCPIv3_0.Facilities.TryParse,
                                              out HashSet<Facilities> Facilities,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse OpeningTimes          [optional]

                if (JSON.ParseOptionalJSON("opening_times",
                                           "opening times",
                                           Hours.TryParse,
                                           out Hours? OpeningTimes,
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
                                           OCPIv3_0.EnergyMix.TryParse,
                                           out EnergyMix? EnergyMix,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EnergyMeters          [optional]

                if (JSON.ParseOptionalHashSet("energy_meters",
                                              "location energy meters",
                                              EnergyMeter<Location>.TryParse,
                                              out HashSet<EnergyMeter<Location>> EnergyMeters,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MaxPower              [optional]

                if (JSON.ParseOptionalJSON("max_power",
                                           "max power",
                                           LocationMaxPower.TryParse,
                                           out LocationMaxPower? MaxPower,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse HelpPhone             [optional]

                if (JSON.ParseOptional("help_phone",
                                       "help phone",
                                       PhoneNumber.TryParse,
                                       out PhoneNumber? HelpPhone,
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

                               PartyIdBody    ?? PartyIdURL!.   Value,
                               LocationIdBody ?? LocationIdURL!.Value,
                               VersionIdBody  ?? VersionIdURL!. Value,

                               Publish,
                               TimeZone,

                               PublishAllowedTo,
                               Name,
                               Address,
                               RelatedLocations,
                               ParkingType,
                               ChargingPool,
                               Directions,
                               Operator,
                               SubOperator,
                               Owner,
                               Services,
                               Facilities,
                               OpeningTimes,
                               ChargingWhenClosed,
                               Images,
                               EnergyMix,
                               EnergyMeters,
                               MaxPower,
                               HelpPhone,

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

        #region ToJSON(CustomLocationSerializer = null, CustomPublishTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeOwnerInformation">Whether to include optional owner information.</param>
        /// <param name="IncludeVersionInformation">Whether to include version information.</param>
        /// <param name="IncludeCreatedTimestamp">Whether to include a timestamp of when this location was created.</param>
        /// <param name="IncludeExtensions">Whether to include optional data model extensions.</param>
        /// <param name="CustomLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomPublishTokenSerializer">A delegate to serialize custom publish token type JSON objects.</param>
        /// <param name="CustomAddressSerializer">A delegate to serialize custom address JSON objects.</param>
        /// <param name="CustomAdditionalGeoLocationSerializer">A delegate to serialize custom additional geo location JSON objects.</param>
        /// <param name="CustomChargingStationSerializer">A delegate to serialize custom charging station JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomParkingSerializer">A delegate to serialize custom parking JSON objects.</param>
        /// <param name="CustomParkingRestrictionSerializer">A delegate to serialize custom parking restriction JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomLocationEnergyMeterSerializer">A delegate to serialize custom location energy meter JSON objects.</param>
        /// <param name="CustomChargingStationEnergyMeterSerializer">A delegate to serialize custom charging station energy meter JSON objects.</param>
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
        /// <param name="CustomLocationMaxPowerSerializer">A delegate to serialize custom location max power JSON objects.</param>
        public JObject ToJSON(Boolean                                                         IncludeOwnerInformation                      = true,
                              Boolean                                                         IncludeVersionInformation                    = true,
                              Boolean                                                         IncludeCreatedTimestamp                      = true,
                              Boolean                                                         IncludeExtensions                            = true,
                              CustomJObjectSerializerDelegate<Location>?                      CustomLocationSerializer                     = null,
                              CustomJObjectSerializerDelegate<PublishToken>?                  CustomPublishTokenSerializer                 = null,
                              CustomJObjectSerializerDelegate<Address>?                       CustomAddressSerializer                      = null,
                              CustomJObjectSerializerDelegate<AdditionalGeoLocation>?         CustomAdditionalGeoLocationSerializer        = null,
                              CustomJObjectSerializerDelegate<ChargingStation>?               CustomChargingStationSerializer              = null,
                              CustomJObjectSerializerDelegate<EVSE>?                          CustomEVSESerializer                         = null,
                              CustomJObjectSerializerDelegate<Parking>?                       CustomParkingSerializer                      = null,
                              CustomJObjectSerializerDelegate<ParkingRestriction>?            CustomParkingRestrictionSerializer           = null,
                              CustomJObjectSerializerDelegate<StatusSchedule>?                CustomStatusScheduleSerializer               = null,
                              CustomJObjectSerializerDelegate<Connector>?                     CustomConnectorSerializer                    = null,
                              CustomJObjectSerializerDelegate<EnergyMeter<Location>>?         CustomLocationEnergyMeterSerializer          = null,
                              CustomJObjectSerializerDelegate<EnergyMeter<ChargingStation>>?  CustomChargingStationEnergyMeterSerializer   = null,
                              CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?             CustomEVSEEnergyMeterSerializer              = null,
                              CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?    CustomTransparencySoftwareStatusSerializer   = null,
                              CustomJObjectSerializerDelegate<TransparencySoftware>?          CustomTransparencySoftwareSerializer         = null,
                              CustomJObjectSerializerDelegate<DisplayText>?                   CustomDisplayTextSerializer                  = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>?               CustomBusinessDetailsSerializer              = null,
                              CustomJObjectSerializerDelegate<Hours>?                         CustomHoursSerializer                        = null,
                              CustomJObjectSerializerDelegate<Image>?                         CustomImageSerializer                        = null,
                              CustomJObjectSerializerDelegate<EnergyMix>?                     CustomEnergyMixSerializer                    = null,
                              CustomJObjectSerializerDelegate<EnergySource>?                  CustomEnergySourceSerializer                 = null,
                              CustomJObjectSerializerDelegate<EnvironmentalImpact>?           CustomEnvironmentalImpactSerializer          = null,
                              CustomJObjectSerializerDelegate<LocationMaxPower>?              CustomLocationMaxPowerSerializer             = null)
        {

            var json = JSONObject.Create(

                           IncludeOwnerInformation
                               ? new JProperty("party_id",               PartyId.         ToString())
                               : null,

                                 new JProperty("id",                     Id.              ToString()),

                           IncludeVersionInformation
                               ? new JProperty("version",                VersionId.       ToString())
                               : null,


                                 new JProperty("publish",                Publish),
                                 new JProperty("time_zone",              Timezone),


                           Publish == false && PublishAllowedTo.Any()
                               ? new JProperty("publish_allowed_to",     new JArray(PublishAllowedTo.   Select(publishTokenType => publishTokenType.ToJSON(CustomPublishTokenSerializer))))
                               : null,

                           Name.IsNotNullOrEmpty()
                               ? new JProperty("name",                   Name)
                               : null,

                           Address is not null
                               ? new JProperty("address",                Address.          ToJSON(CustomAddressSerializer))
                               : null,

                           RelatedLocations.Any()
                               ? new JProperty("related_locations",      new JArray(RelatedLocations.   Select (additionalGeoLocation => additionalGeoLocation.ToJSON(CustomAdditionalGeoLocationSerializer,
                                                                                                                                                                      CustomDisplayTextSerializer))))
                               : null,

                           ParkingType.HasValue
                               ? new JProperty("parking_type",           ParkingType.Value.ToString())
                               : null,

                           ChargingPool.Any()
                               ? new JProperty("charging_pool",          new JArray(ChargingPool.       OrderBy(chargingStation       => chargingStation.Id).
                                                                                                        Select (chargingStation       => chargingStation.ToJSON(true,
                                                                                                                                                                true,
                                                                                                                                                                CustomChargingStationSerializer,
                                                                                                                                                                CustomEVSESerializer,
                                                                                                                                                                CustomConnectorSerializer,
                                                                                                                                                                CustomParkingSerializer,
                                                                                                                                                                CustomParkingRestrictionSerializer,
                                                                                                                                                                CustomImageSerializer,
                                                                                                                                                                CustomStatusScheduleSerializer,
                                                                                                                                                                CustomChargingStationEnergyMeterSerializer,
                                                                                                                                                                CustomEVSEEnergyMeterSerializer,
                                                                                                                                                                CustomTransparencySoftwareStatusSerializer,
                                                                                                                                                                CustomTransparencySoftwareSerializer,
                                                                                                                                                                CustomDisplayTextSerializer))))
                               : null,

                           Directions.Any()
                               ? new JProperty("directions",             new JArray(Directions.         Select (displayText           => displayText.ToJSON(CustomDisplayTextSerializer))))
                               : null,

                           Operator is not null
                               ? new JProperty("operator",               Operator.         ToJSON(CustomBusinessDetailsSerializer))
                               : null,

                           SubOperator is not null
                               ? new JProperty("suboperator",            SubOperator.      ToJSON(CustomBusinessDetailsSerializer))
                               : null,

                           Owner is not null
                               ? new JProperty("owner",                  Owner.            ToJSON(CustomBusinessDetailsSerializer))
                               : null,

                           Services.Any()
                               ? new JProperty("services",               new JArray(Services.           Select (service               => service.ToString())))
                               : null,

                           Facilities.Any()
                               ? new JProperty("facilities",             new JArray(Facilities.         Select (facility              => facility.ToString())))
                               : null,

                           OpeningTimes is not null
                               ? new JProperty("opening_times",          OpeningTimes.     ToJSON(CustomHoursSerializer))
                               : null,

                           ChargingWhenClosed.HasValue
                               ? new JProperty("charging_when_closed",   ChargingWhenClosed.Value)
                               : null,

                           Images.Any()
                               ? new JProperty("images",                 new JArray(Images.             Select (image                 => image.ToJSON(CustomImageSerializer))))
                               : null,

                           EnergyMix is not null
                               ? new JProperty("energy_mix",             EnergyMix.        ToJSON(CustomEnergyMixSerializer,
                                                                                                  CustomEnergySourceSerializer,
                                                                                                  CustomEnvironmentalImpactSerializer))
                               : null,

                           energyMeters.Values.Count != 0
                               ? new JProperty("energy_meters",          new JArray(EnergyMeters.OrderBy(energyMeter           => energyMeter.Id).
                                                                                                 Select (energyMeter           => energyMeter.ToJSON(CustomLocationEnergyMeterSerializer,
                                                                                                                                                     CustomTransparencySoftwareStatusSerializer,
                                                                                                                                                     CustomTransparencySoftwareSerializer))))
                               : null,

                           MaxPower is not null
                               ? new JProperty("max_power",              MaxPower. Value.  ToJSON(CustomLocationMaxPowerSerializer))
                               : null,

                           HelpPhone.HasValue
                               ? new JProperty("help_phone",             HelpPhone.Value.  ToString())
                               : null,


                           IncludeCreatedTimestamp
                               ? new JProperty("created",                Created.          ToIso8601())
                               : null,

                                 new JProperty("last_updated",           LastUpdated.      ToIso8601())

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
                   PartyId.         Clone(),
                   Id.              Clone(),
                   VersionId,

                   Publish,
                   Timezone.        CloneString(),

                   PublishAllowedTo.Select(publishToken    => publishToken.   Clone()),
                   Name.            CloneNullableString(),
                   Address?.        Clone(),
                   RelatedLocations.Select(relatedLocation => relatedLocation.Clone()),
                   ParkingType?.    Clone(),
                   ChargingPool.    Select(chargingStation => chargingStation.Clone()),
                   Directions.      Select(displayText     => displayText.    Clone()),
                   Operator?.       Clone(),
                   SubOperator?.    Clone(),
                   Owner?.          Clone(),
                   Services.        Select(service         => service.        Clone()),
                   Facilities.      Select(facility        => facility.       Clone()),
                   OpeningTimes?.   Clone(),
                   ChargingWhenClosed,
                   Images.          Select(image           => image.          Clone()),
                   EnergyMix?.      Clone(),
                   EnergyMeters,
                   MaxPower?.       Clone(),
                   HelpPhone?.      Clone(),

                   Created,
                   LastUpdated

               );

        #endregion


        #region (private, static) TryPrivatePatch(JSON, Patch)

        private static PatchResult<JObject> TryPrivatePatch(JObject           JSON,
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

            if (LocationPatch is null)
                return PatchResult<Location>.Failed(EventTrackingId, this,
                                                    "The given location patch must not be null!");

            lock (patchLock)
            {

                if (LocationPatch["last_updated"] is null)
                    LocationPatch["last_updated"] = Timestamp.Now.ToIso8601();

                else if (AllowDowngrades == false &&
                        LocationPatch["last_updated"].Type == JTokenType.Date &&
                       (LocationPatch["last_updated"].Value<DateTime>().ToIso8601().CompareTo(LastUpdated.ToIso8601()) < 1))
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
                             out var errorResponse) &&
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


        #region (internal) TryAddChargingStation(ChargingStation)

        internal void TryAddChargingStation(ChargingStation ChargingStation)
        {

            if (chargingPool.TryAdd(ChargingStation.Id, ChargingStation))
            {

            }
            else
            {
            }

        }

        #endregion

        #region ChargingStationExists(ChargingStationId)

        /// <summary>
        /// Checks whether any charging station having the given charging station identification exists.
        /// </summary>
        /// <param name="ChargingStationId">A charging station identification.</param>
        public Boolean ChargingStationExists(ChargingStation_Id ChargingStationId)

            => chargingPool.ContainsKey(ChargingStationId);

        #endregion

        #region TryGetChargingStation(ChargingStationId, out ChargingStation)

        /// <summary>
        /// Try to return the charging station having the given charging station identification.
        /// </summary>
        /// <param name="ChargingStationId">A charging station identification.</param>
        /// <param name="ChargingStation">The charging station having the given charging station identification.</param>
        public Boolean TryGetChargingStation(ChargingStation_Id                        ChargingStationId,
                                             [NotNullWhen(true)] out ChargingStation?  ChargingStation)
        {

            if (chargingPool.TryGetValue(ChargingStationId, out ChargingStation))
            {
                return true;
            }

            ChargingStation = null;
            return false;

        }

        #endregion

        #region IEnumerable<ChargingStation> Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => ChargingPool.GetEnumerator();

        public IEnumerator<ChargingStation> GetEnumerator()
            => ChargingPool.GetEnumerator();

        #endregion

        #region (internal) RemoveChargingStation(ChargingStation)

        internal void RemoveChargingStation(ChargingStation ChargingStation)
        {

            if (chargingPool.TryRemove(ChargingStation.Id, out _))
            {

            }
            else
            {
            }

        }

        #endregion

        #region (internal) RemoveChargingStation(ChargingStationId)

        internal void RemoveChargingStation(ChargingStation_Id ChargingStationId)
        {

            if (chargingPool.TryRemove(ChargingStationId, out _))
            {

            }
            else
            {
            }

        }

        #endregion



        #region EnergyMeterExists(EnergyMeterId)

        /// <summary>
        /// Checks whether any charging station having the given charging station identification exists.
        /// </summary>
        /// <param name="EnergyMeterId">A charging station identification.</param>
        public Boolean EnergyMeterExists(EnergyMeter_Id EnergyMeterId)

            => energyMeters.ContainsKey(EnergyMeterId);

        #endregion

        #region TryGetEnergyMeter(EnergyMeterId, out EnergyMeter)

        /// <summary>
        /// Try to return the charging station having the given charging station identification.
        /// </summary>
        /// <param name="EnergyMeterId">A charging station identification.</param>
        /// <param name="EnergyMeter">The charging station having the given charging station identification.</param>
        public Boolean TryGetEnergyMeter(EnergyMeter_Id                                      EnergyMeterId,
                                             [NotNullWhen(true)] out EnergyMeter<Location>?  EnergyMeter)
        {

            if (energyMeters.TryGetValue(EnergyMeterId, out EnergyMeter))
            {
                return true;
            }

            EnergyMeter = null;
            return false;

        }

        #endregion




        #region CalcSHA256Hash(CustomLocationSerializer = null, CustomPublishTokenSerializer = null, ...)

        /// <summary>
        /// Return the SHA256 hash of the JSON representation of this location as Base64.
        /// </summary>
        /// <param name="CustomLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomPublishTokenSerializer">A delegate to serialize custom publish token type JSON objects.</param>
        /// <param name="CustomAddressSerializer">A delegate to serialize custom address JSON objects.</param>
        /// <param name="CustomAdditionalGeoLocationSerializer">A delegate to serialize custom additional geo location JSON objects.</param>
        /// <param name="CustomChargingStationSerializer">A delegate to serialize custom ChargingStation JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomParkingSerializer">A delegate to serialize custom parking JSON objects.</param>
        /// <param name="CustomParkingRestrictionSerializer">A delegate to serialize custom parking restriction JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomLocationEnergyMeterSerializer">A delegate to serialize custom location energy meter JSON objects.</param>
        /// <param name="CustomChargingStationEnergyMeterSerializer">A delegate to serialize custom charging station energy meter JSON objects.</param>
        /// <param name="CustomEVSEEnergyMeterSerializer">A delegate to serialize custom EVSE energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomHoursSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        public String CalcSHA256Hash(CustomJObjectSerializerDelegate<Location>?                      CustomLocationSerializer                     = null,
                                     CustomJObjectSerializerDelegate<PublishToken>?                  CustomPublishTokenSerializer                 = null,
                                     CustomJObjectSerializerDelegate<Address>?                       CustomAddressSerializer                      = null,
                                     CustomJObjectSerializerDelegate<AdditionalGeoLocation>?         CustomAdditionalGeoLocationSerializer        = null,
                                     CustomJObjectSerializerDelegate<ChargingStation>?               CustomChargingStationSerializer              = null,
                                     CustomJObjectSerializerDelegate<EVSE>?                          CustomEVSESerializer                         = null,
                                     CustomJObjectSerializerDelegate<Connector>?                     CustomConnectorSerializer                    = null,
                                     CustomJObjectSerializerDelegate<Parking>?                       CustomParkingSerializer                      = null,
                                     CustomJObjectSerializerDelegate<ParkingRestriction>?            CustomParkingRestrictionSerializer           = null,
                                     CustomJObjectSerializerDelegate<Image>?                         CustomImageSerializer                        = null,
                                     CustomJObjectSerializerDelegate<StatusSchedule>?                CustomStatusScheduleSerializer               = null,
                                     CustomJObjectSerializerDelegate<EnergyMeter<Location>>?         CustomLocationEnergyMeterSerializer          = null,
                                     CustomJObjectSerializerDelegate<EnergyMeter<ChargingStation>>?  CustomChargingStationEnergyMeterSerializer   = null,
                                     CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?             CustomEVSEEnergyMeterSerializer              = null,
                                     CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?    CustomTransparencySoftwareStatusSerializer   = null,
                                     CustomJObjectSerializerDelegate<TransparencySoftware>?          CustomTransparencySoftwareSerializer         = null,
                                     CustomJObjectSerializerDelegate<DisplayText>?                   CustomDisplayTextSerializer                  = null,
                                     CustomJObjectSerializerDelegate<BusinessDetails>?               CustomBusinessDetailsSerializer              = null,
                                     CustomJObjectSerializerDelegate<Hours>?                         CustomHoursSerializer                        = null,
                                     CustomJObjectSerializerDelegate<EnergyMix>?                     CustomEnergyMixSerializer                    = null,
                                     CustomJObjectSerializerDelegate<EnergySource>?                  CustomEnergySourceSerializer                 = null,
                                     CustomJObjectSerializerDelegate<EnvironmentalImpact>?           CustomEnvironmentalImpactSerializer          = null)
        {

            ETag = SHA256.HashData(
                       ToJSON(
                           true,
                           true,
                           true,
                           true,
                           CustomLocationSerializer,
                           CustomPublishTokenSerializer,
                           CustomAddressSerializer,
                           CustomAdditionalGeoLocationSerializer,
                           CustomChargingStationSerializer,
                           CustomEVSESerializer,
                           CustomParkingSerializer,
                           CustomParkingRestrictionSerializer,
                           CustomStatusScheduleSerializer,
                           CustomConnectorSerializer,
                           CustomLocationEnergyMeterSerializer,
                           CustomChargingStationEnergyMeterSerializer,
                           CustomEVSEEnergyMeterSerializer,
                           CustomTransparencySoftwareStatusSerializer,
                           CustomTransparencySoftwareSerializer,
                           CustomDisplayTextSerializer,
                           CustomBusinessDetailsSerializer,
                           CustomHoursSerializer,
                           CustomImageSerializer,
                           CustomEnergyMixSerializer,
                           CustomEnergySourceSerializer,
                           CustomEnvironmentalImpactSerializer
                       ).ToUTF8Bytes(Newtonsoft.Json.Formatting.None)
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
                   : throw new ArgumentException("The given object is not a location!",
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
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            var c = PartyId.    CompareTo(Location.PartyId);

            if (c == 0)
                c = Id.         CompareTo(Location.Id);

            if (c == 0)
                c = VersionId.  CompareTo(Location.VersionId);


            if (c == 0)
                c = Created.    CompareTo(Location.Created);

            if (c == 0)
                c = LastUpdated.CompareTo(Location.LastUpdated);

            if (c == 0)
                c = ETag.       CompareTo(Location.ETag);

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

               PartyId.                Equals(Location.PartyId)                 &&
               Id.                     Equals(Location.Id)                      &&
               VersionId.              Equals(Location.VersionId)               &&

               Publish.                Equals(Location.Publish)                 &&
               Timezone.               Equals(Location.Timezone)                &&

               Created.    ToIso8601().Equals(Location.Created.    ToIso8601()) &&
               LastUpdated.ToIso8601().Equals(Location.LastUpdated.ToIso8601()) &&

             ((Name               is     null &&  Location.Name               is     null) ||
              (Name               is not null &&  Location.Name               is not null && Name.                 Equals(Location.Name)))                     &&

             ((Address            is     null &&  Location.Address            is     null) ||
              (Address            is not null &&  Location.Address            is not null && Address.              Equals(Location.Address)))                  &&

            ((!ParkingType.       HasValue    && !Location.ParkingType.       HasValue)    ||
              (ParkingType.       HasValue    &&  Location.ParkingType.       HasValue && ParkingType.       Value.Equals(Location.ParkingType.       Value))) &&

             ((Operator           is     null &&  Location.Operator           is     null) ||
              (Operator           is not null &&  Location.Operator           is not null && Operator.             Equals(Location.Operator)))                 &&

             ((SubOperator        is     null &&  Location.SubOperator        is     null) ||
              (SubOperator        is not null &&  Location.SubOperator        is not null && SubOperator.          Equals(Location.SubOperator)))              &&

             ((Owner              is     null &&  Location.Owner              is     null) ||
              (Owner              is not null &&  Location.Owner              is not null && Owner.                Equals(Location.Owner)))                    &&

             ((OpeningTimes       is     null &&  Location.OpeningTimes       is     null) ||
              (OpeningTimes       is not null &&  Location.OpeningTimes       is not null && OpeningTimes.         Equals(Location.OpeningTimes)))             &&

            ((!ChargingWhenClosed.HasValue    && !Location.ChargingWhenClosed.HasValue)    ||
              (ChargingWhenClosed.HasValue    &&  Location.ChargingWhenClosed.HasValue && ChargingWhenClosed.Value.Equals(Location.ChargingWhenClosed.Value))) &&

             ((EnergyMix          is     null &&  Location.EnergyMix          is     null) ||
              (EnergyMix          is not null &&  Location.EnergyMix          is not null && EnergyMix.            Equals(Location.EnergyMix)))                &&

               PublishAllowedTo.Count().Equals(Location.PublishAllowedTo.Count()) &&
               RelatedLocations.Count().Equals(Location.RelatedLocations.Count()) &&
               ChargingPool.    Count().Equals(Location.ChargingPool.    Count()) &&
               Directions.      Count().Equals(Location.Directions.      Count()) &&
               Facilities.      Count().Equals(Location.Facilities.      Count()) &&
               Images.          Count().Equals(Location.Images.          Count()) &&

               PublishAllowedTo.All(publishTokenType      => Location.PublishAllowedTo.Contains(publishTokenType))      &&
               RelatedLocations.All(additionalGeoLocation => Location.RelatedLocations.Contains(additionalGeoLocation)) &&
               ChargingPool.    All(evse                  => Location.ChargingPool.    Contains(evse))                  &&
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

            => $"{PartyId}:{Id} ({VersionId}, {LastUpdated.ToIso8601()}) {ChargingPool.Count()} Charging Station(s), ";

        #endregion


        #region ToBuilder(NewLocationId = null, NewVersionId = null)

        /// <summary>
        /// Return a builder for this location.
        /// </summary>
        /// <param name="NewLocationId">An optional new location identification.</param>
        /// <param name="NewVersionId">An optional new version identification.</param>
        public Builder ToBuilder(Location_Id? NewLocationId  = null,
                                 UInt64?      NewVersionId   = null)

            => new (

                   CommonAPI,
                   PartyId,
                   NewLocationId ?? Id,
                   NewVersionId  ?? VersionId,

                   Publish,
                   Timezone,

                   PublishAllowedTo,
                   Name,
                   Address,
                   RelatedLocations,
                   ParkingType,
                   ChargingPool,
                   Directions,
                   Operator,
                   SubOperator,
                   Owner,
                   Services,
                   Facilities,
                   OpeningTimes,
                   ChargingWhenClosed,
                   Images,
                   EnergyMix,
                   EnergyMeters,
                   MaxPower,
                   HelpPhone,

                   Created,
                   LastUpdated

               );

        #endregion

        #region (class) Builder

        /// <summary>
        /// A location builder.
        /// </summary>
        public class Builder : ABuilder
        {

            #region Properties

            /// <summary>
            /// Whether the receiving Party or Platform may publish the Location.
            /// When this is set to false, the receiving Party or Platform MAY NOT disclose information from this Location object
            /// to anyone not holding a Token listed in the field publish_allowed_to.
            /// When the same physical facility has both some EVSEs that may be published and other ones that may not be published,
            /// the sender Party SHOULD send two separate Location objects for the two groups of EVSEs.
            /// </summary>
            [Mandatory]
            public Boolean?                        Publish                  { get; set; }

            /// <summary>
            /// One of IANA tzdata’s TZ-values representing the time zone of the location (http://www.iana.org/time-zones).
            /// </summary>
            /// <example>"Europe/Oslo", "Europe/Zurich"</example>
            [Mandatory]
            public String?                         Timezone                 { get; set; }

            /// <summary>
            /// This field SHALL NOT be used unless the publish field is set to false.
            /// Only holders of Tokens that match all the set fields of one PublishToken in the list are allowed to be shown this Location.
            /// </summary>
            [Optional]
            public HashSet<PublishToken>           PublishAllowedTo         { get; } = [];

            /// <summary>
            /// Display name of the Location.
            /// </summary>
            [Optional]
            public String?                         Name                     { get; set; }

            /// <summary>
            /// Address and geographical location of the Location.
            /// This has to be present unless the publish field is set to false.
            /// </summary>
            [Optional]
            public Address?                        Address                  { get; set; }

            /// <summary>
            /// The optional enumeration of geographical locations of related points relevant to the user.
            /// </summary>
            [Optional]
            public HashSet<AdditionalGeoLocation>  RelatedLocations         { get; } = [];

            /// <summary>
            /// The optional general type of parking at the location.
            /// </summary>
            [Optional]
            public ParkingType?                    ParkingType              { get; set; }

            /// <summary>
            /// The Charging Pool of this Location, that is, the list of Charging Stations
            /// that make up the physical charging infrastructure of this Location.
            /// </summary>
            [Mandatory]
            public ConcurrentDictionary<ChargingStation_Id, ChargingStation>  ChargingPool    { get; } = new ();

            /// <summary>
            /// The optional enumeration of human-readable directions on how to reach the location.
            /// </summary>
            [Optional]
            public HashSet<DisplayText>            Directions               { get; } = [];

            /// <summary>
            /// Information of the operator. When not specified, the information retrieved with Use Case Request Parties Served by Platform,
            /// selected by the Party ID of the Party that issued this Location, MAY be used instead.
            /// </summary>
            [Optional]
            public BusinessDetails?                Operator                 { get; set; }

            /// <summary>
            /// Optional information about the suboperator.
            /// </summary>
            [Optional]
            public BusinessDetails?                SubOperator              { get; set; }

            /// <summary>
            /// Optional information about the owner.
            /// </summary>
            [Optional]
            public BusinessDetails?                Owner                    { get; set; }

            /// <summary>
            /// The optional enumeration of services that are offered at the Location by the CPO or their affiliated partners.
            /// </summary>
            [Optional]
            public HashSet<LocationService>        Services                 { get; } = [];

            /// <summary>
            /// The optional enumeration of facilities this location directly belongs to.
            /// </summary>
            [Optional]
            public HashSet<Facilities>             Facilities               { get; } = [];

            /// <summary>
            /// The optional times when the EVSEs at the location can be accessed for charging.
            /// </summary>
            [Optional]
            public Hours?                          OpeningTimes             { get; set; }

            /// <summary>
            /// Indicates if the EVSEs are still charging outside the opening
            /// hours of the location. E.g. when the parking garage closes its
            /// barriers over night, is it allowed to charge till the next
            /// morning? [Default: true]
            /// </summary>
            [Optional]
            public Boolean?                        ChargingWhenClosed       { get; set; }

            /// <summary>
            /// The optional enumeration of images related to the location such as photos or logos.
            /// </summary>
            [Optional]
            public HashSet<Image>                  Images                   { get; } = [];

            /// <summary>
            /// Optional details on the energy supplied at this location.
            /// </summary>
            [Optional]
            public EnergyMix?                      EnergyMix                { get; set; }

            /// <summary>
            /// The optional enumeration of energy meters at this location, e.g. at the grid connection point.
            /// </summary>
            [Optional]
            public HashSet<EnergyMeter<Location>>  EnergyMeters             { get; } = [];

            /// <summary>
            /// How much power or current this Location can draw from the grid at any one time.
            /// </summary>
            [Optional]
            public LocationMaxPower?               MaxPower                 { get; set; }

            /// <summary>
            /// A telephone number that a Driver using the Location may call for assistance.Calling this number will typically
            /// connect the caller to the CPO’s customer service department.
            /// </summary>
            [Optional]
            public PhoneNumber?                    HelpPhone                { get; set; }

            /// <summary>
            /// The timestamp when this location was created.
            /// </summary>
            [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
            public DateTime?                       Created                  { get; set; }

            /// <summary>
            /// The timestamp when this location was last updated (or created).
            /// </summary>
            [Mandatory]
            public DateTime?                       LastUpdated              { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new location builder.
            /// </summary>
            /// <param name="PartyId">The party identification of the party that issued this location.</param>
            /// <param name="Id">An identification of the location within the party.</param>
            /// <param name="VersionId">The version identification of the location.</param>
            /// 
            /// <param name="Publish">Whether this location may be published on an website or app etc., or not.</param>
            /// <param name="Timezone">One of IANA tzdata’s TZ-values representing the time zone of the location (http://www.iana.org/time-zones).</param>
            /// 
            /// <param name="PublishAllowedTo">An optional enumeration of publish tokens. Only owners of tokens that match all the set fields of one publish token in the list are allowed to be shown this location.</param>
            /// <param name="Name">An optional display name of the location.</param>
            /// <param name="Address">The address of the location.</param>
            /// <param name="RelatedLocations">An optional enumeration of additional geographical locations of related geo coordinates that might be relevant to the EV driver.</param>
            /// <param name="ParkingType">An optional general type of parking at the location.</param>
            /// <param name="ChargingPool">An optional enumeration of charging stations at this location.</param>
            /// <param name="Directions">An optional enumeration of human-readable directions on how to reach the location.</param>
            /// <param name="Operator">Optional information about the charging station operator.</param>
            /// <param name="SubOperator">Optional information about the suboperator.</param>
            /// <param name="Owner">Optional information about the owner.</param>
            /// <param name="Services">An optional enumeration of services that are offered at the Location by the CPO or their affiliated partners.</param>
            /// <param name="Facilities">An optional enumeration of facilities this location directly belongs to.</param>
            /// <param name="OpeningTimes">An optional times when the EVSEs at the location can be accessed for charging.</param>
            /// <param name="ChargingWhenClosed">Indicates if the EVSEs are still charging outside the opening hours of the location. </param>
            /// <param name="Images">An optional enumeration of images related to the location such as photos or logos.</param>
            /// <param name="EnergyMix">Optional details on the energy supplied at this location.</param>
            /// 
            /// <param name="MaxPower">How much power or current this Location can draw from the grid at any one time.</param>
            /// <param name="HelpPhone">A telephone number that a Driver using the Location may call for assistance. Calling this number will typically connect the caller to the CPO’s customer service department.</param>
            /// 
            /// <param name="Created">An optional timestamp when this location was created.</param>
            /// <param name="LastUpdated">An optional timestamp when this location was last updated (or created).</param>
            public Builder(CommonAPI?                           CommonAPI            = null,
                           Party_Idv3?                          PartyId              = null,
                           Location_Id?                         Id                   = null,
                           UInt64?                              VersionId            = null,

                           Boolean?                             Publish              = null,
                           String?                              Timezone             = null,

                           IEnumerable<PublishToken>?           PublishAllowedTo     = null,
                           String?                              Name                 = null,
                           Address?                             Address              = null,
                           IEnumerable<AdditionalGeoLocation>?  RelatedLocations     = null,
                           ParkingType?                         ParkingType          = null,
                           IEnumerable<ChargingStation>?        ChargingPool         = null,
                           IEnumerable<DisplayText>?            Directions           = null,
                           BusinessDetails?                     Operator             = null,
                           BusinessDetails?                     SubOperator          = null,
                           BusinessDetails?                     Owner                = null,
                           IEnumerable<LocationService>?        Services             = null,
                           IEnumerable<Facilities>?             Facilities           = null,
                           Hours?                               OpeningTimes         = null,
                           Boolean?                             ChargingWhenClosed   = null,
                           IEnumerable<Image>?                  Images               = null,
                           EnergyMix?                           EnergyMix            = null,
                           IEnumerable<EnergyMeter<Location>>?  EnergyMeters         = null,
                           LocationMaxPower?                    MaxPower             = null,
                           PhoneNumber?                         HelpPhone            = null,

                           DateTime?                            Created              = null,
                           DateTime?                            LastUpdated          = null)

                : base(CommonAPI,
                       PartyId,
                       Id,
                       VersionId)

            {

                this.Publish             = Publish;
                this.Timezone            = Timezone;

                this.PublishAllowedTo    = PublishAllowedTo is not null ? new HashSet<PublishToken>         (PublishAllowedTo) : [];
                this.Name                = Name;
                this.Address             = Address;
                this.RelatedLocations    = RelatedLocations is not null ? new HashSet<AdditionalGeoLocation>(RelatedLocations) : [];
                this.ParkingType         = ParkingType;
                this.Directions          = Directions       is not null ? new HashSet<DisplayText>          (Directions)       : [];
                this.Operator            = Operator;
                this.SubOperator         = SubOperator;
                this.Owner               = Owner;
                this.Services            = Services         is not null ? new HashSet<LocationService>      (Services)         : [];
                this.Facilities          = Facilities       is not null ? new HashSet<Facilities>           (Facilities)       : [];
                this.OpeningTimes        = OpeningTimes;
                this.ChargingWhenClosed  = ChargingWhenClosed;
                this.Images              = Images           is not null ? new HashSet<Image>                (Images)           : [];
                this.EnergyMix           = EnergyMix;
                this.EnergyMeters        = EnergyMeters     is not null ? new HashSet<EnergyMeter<Location>>(EnergyMeters)     : [];
                this.MaxPower            = MaxPower;
                this.HelpPhone           = HelpPhone;

                this.Created             = Created;
                this.LastUpdated         = LastUpdated;

                foreach (var chargingStation in ChargingPool?.Distinct() ?? [])
                {

                    chargingStation.ParentLocation = this;

                    this.ChargingPool.TryAdd(
                        chargingStation.Id,
                        chargingStation
                    );

                }

            }

            #endregion

            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the location.
            /// </summary>
            public static implicit operator Location(Builder Builder)

                => Builder.ToImmutable;


            /// <summary>
            /// Return an immutable version of the location.
            /// </summary>
            public Location ToImmutable
            {
                get
                {

                    if (!PartyId.    HasValue)
                        throw new ArgumentNullException(nameof(PartyId),    "The party identification of the location must not be null or empty!");

                    if (!Id.         HasValue)
                        throw new ArgumentNullException(nameof(Id),         "The location identification must not be null or empty!");

                    if (!VersionId.  HasValue)
                        throw new ArgumentNullException(nameof(VersionId),  "The version identification of the location must not be null or empty!");

                    if (!Publish.    HasValue)
                        throw new ArgumentNullException(nameof(Publish),    "The publish parameter must not be null or empty!");

                    Timezone = Timezone?.Trim();

                    if (Timezone.IsNullOrEmpty())
                        throw new ArgumentNullException(nameof(Timezone),   "The time zone parameter must not be null or empty!");


                    return new Location(

                               CommonAPI,
                               PartyId.    Value,
                               Id.         Value,
                               VersionId.  Value,

                               Publish.    Value,
                               Timezone,

                               PublishAllowedTo,
                               Name,
                               Address,
                               RelatedLocations,
                               ParkingType,
                               ChargingPool.Values,
                               Directions,
                               Operator,
                               SubOperator,
                               Owner,
                               Services,
                               Facilities,
                               OpeningTimes,
                               ChargingWhenClosed,
                               Images,
                               EnergyMix,
                               EnergyMeters,
                               MaxPower,
                               HelpPhone,

                               Created     ?? Timestamp.Now,
                               LastUpdated ?? Timestamp.Now

                           );

                }
            }

            #endregion

        }

        #endregion


    }

}
