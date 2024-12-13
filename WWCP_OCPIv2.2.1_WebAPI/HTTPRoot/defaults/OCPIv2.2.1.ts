/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI WebAPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
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

// ------------------------------------------------------------
//  OCPI v2.2.1
//  https://github.com/ocpi/ocpi/blob/release-2.2.1-bugfixes/
//
//  Keep it in sync with:
//  https://github.com/OpenChargingCloud/OCPIExplorerWebApp
//  https://github.com/OpenChargingCloud/OCPIExplorerDesktopApp
// ------------------------------------------------------------

///<reference path="defaults.ts" />



type VersionNumber =
    "2.0"   |
    "2.1"   | // DEPRECATED, do not use, use 2.1.1 instead
    "2.1.1" |
    "2.2"   | // DEPRECATED, do not use, use 2.2.1 instead
    "2.2.1" |
    "2.3"   |
    "3.0"   |
     string;

type ModuleId =
    "cdrs"             |
    "chargingprofiles" |
    "commands"         |
    "credentials"      |
    "hubclientinfo"    |
    "locations"        |
    "sessions"         |
    "tariffs"          |
    "tokens"           |
     string;

type InterfaceRole =
    "SENDER" |
    "RECEIVER";

interface IVersion {
    version:                         VersionNumber;                 // The version number.
    url:                             string;                        // URL to the endpoint containing version specific information.
}

interface IVersionDetail {
    version:                         VersionNumber;                 // The version number.
    endpoints:                       Array<IEndpoint>;              // A list of supported endpoints for this version.
}

interface IEndpoint {
    identifier:                     ModuleId;                       // Endpoint identifier.
    role:                           InterfaceRole;                  // Interface role this endpoint implements.
    url:                            string;                         // URL to the endpoint.
}

interface IRegularHours {
    weekday:                        number;                         // Number of day in the week, from Monday (1) till Sunday (7)
    period_begin:                   string;                         // Begin of the regular period, in local time, given in hours and minutes. Must be in 24h format with leading zeros. Example: "18:15". Hour/Minute separator: ":" Regex: ([0-1][0-9]|2[0-3]):[0-5][0-9].
    period_end:                     string;                         // End of the regular period, in local time, syntax as for period_begin. Must be later than period_begin.
}
interface IExceptionalPeriod {
    period_begin:                   string;                         // Begin of the exception. In UTC, time_zone field can be used to convert to local time.
    period_end:                     string;                         // End of the exception. In UTC, time_zone field can be used to convert to local time.
}

interface IHours {
    twentyfourseven:                boolean;                        // True to represent 24 hours a day and 7 days a week, except the given exceptions.
    regular_hours?:                 Array<IRegularHours>;           // Regular hours, weekday-based. Only to be used if twentyfourseven=false, then this field needs to contain at least one RegularHours object.
    exceptional_openings?:          Array<IExceptionalPeriod>;      // Exceptions for specified calendar dates, time-range based. Periods the station is operating/accessible. Additional to regular_hours. May overlap regular rules.
    exceptional_closings?:          Array<IExceptionalPeriod>;      // Exceptions for specified calendar dates, time-range based. Periods the station is not operating/accessible. Additional to regular_hours. May overlap regular rules.
}

interface ILocation {
    country_code:                   string;                         // ISO-3166 alpha-2 country code of the CPO that 'owns' this Location.
    party_id:                       string;                         // ID of the CPO that 'owns' this Location (following the ISO-15118 standard).
    id:                             string;                         // Uniquely identifies the location within the CPOs platform (and suboperator platforms). This field can never be changed, modified or renamed.
    publish:                        boolean;                        // Defines if a Location may be published on an website or app etc. When this is set to false, only tokens identified in the field: publish_allowed_to are allowed to be shown this Location. When the same location has EVSEs that may be published and may not be published, two 'Locations' should be created.
    publish_allowed_to?:            Array<IPublishToken>;           // This field may only be used when the publish field is set to false. Only owners of Tokens that match all the set fields of one PublishToken in the list are allowed to be shown this location.
    name?:                          string;                         // Display name of the location.
    address:                        string;                         // Street/block name and house number if available.
    city:                           string;                         // City or town.
    postal_code?:                   string;                         // Postal code of the location, may only be omitted when the location has no postal code: in some countries charging locations at highways don’t have postal codes.
    state?:                         string;                         // State or province of the location, only to be used when relevant.
    country:                        string;                         // ISO 3166-1 alpha-3 code for the country of this location.
    coordinates:                    IGeoLocation;                   // Coordinates of the location.
    related_locations?:             Array<IAdditionalGeoLocation>;  // Geographical location of related points relevant to the user. For example, a location of a restaurant nearby.
    parking_type?:                  ParkingType;                    // The general type of parking at the charge point location.
    evses?:                         Array<IEVSE>;                   // List of EVSEs that belong to this Location.
    directions?:                    Array<IDisplayText>;            // Human-readable directions on how to reach the location.
    operator?:                      IBusinessDetails;               // Information of the operator. When not specified, the information retrieved from the Credentials module, selected by the country_code and party_id of this Location, should be used instead.
    suboperator?:                   IBusinessDetails;               // Information of the suboperator if available.
    owner?:                         IBusinessDetails;               // Information of the owner if available.
    facilities?:                    Array<Facility>;                // Optional list of facilities this charging location directly belongs to.
    time_zone:                      string;                         // One of IANA tzdata’s TZ-values representing the time zone of the location. Examples: "Europe/Oslo", "Europe/Zurich". (http://www.iana.org/time-zones)
    opening_times?:                 IHours;                         // The times when the EVSEs at the location can be accessed for charging.
    charging_when_closed?:          boolean;                        // Indicates if the EVSEs are still charging outside the opening hours of the location. E.g. when the parking garage closes its barriers over night, is it allowed to charge till the next morning? Default: true
    images?:                        Array<IImage>;                  // Links to images related to the location such as photos or logos.
    energy_mix?:                    IEnergyMix;                     // Details on the energy supplied at this location.
    created?:                       string;                         // Optional timestamp when this location was created [OCPI Computer Science Extension!]
    last_updated:                   string;                         // Timestamp when this location or one of its EVSEs or connectors were last updated (or created).
}

interface ILocationMetadata extends TMetadataDefaults {

}

interface IGeoLocation {
    latitude:                       string;                         // Latitude of the point in decimal degree. Example: 50.770774. Decimal separator: "." Regex: -?[0-9]{1,2}\.[0-9]{5,7}
    longitude:                      string;                         // Longitude of the point in decimal degree. Example: -126.104965. Decimal separator: "." Regex: -?[0-9]{1,3}\.[0-9]{5,7}
}

interface IAdditionalGeoLocation {
    latitude:                       string;                         // Latitude of the point in decimal degree. Example: 50.770774. Decimal separator: "." Regex: -?[0-9]{1,2}\.[0-9]{5,7}
    longitude:                      string;                         // Longitude of the point in decimal degree. Example: -126.104965. Decimal separator: "." Regex: -?[0-9]{1,3}\.[0-9]{5,7}
    name?:                          IDisplayText                    // Name of the point in local language or as written at the location. For example the street name of a parking lot entrance or it’s number.
}

interface IDisplayText {
    language:                       string;                         // Language Code ISO 639-1.
    text:                           string;                         // Text to be displayed to a end user. No markup, html etc. allowed.
}

interface IBusinessDetails {
    name:                           string;                         // Name of the operator.
    website?:                       string;                         // Link to the operator’s website.
    logo?:                          string;                         // Image link to the operator’s logo.
}

interface IImage {
    url:                            string;                         // URL from where the image data can be fetched through a web browser.
    thumbnail?:                     string;                         // URL from where a thumbnail of the image can be fetched through a webbrowser.
    category:                       ImageCategory;                  // Describes what the image is used for.
    type:                           string;                         // Image type like: gif, jpeg, png, svg.
    width?:                         string;                         // Width of the full scale image.
    height?:                        string;                         // Height of the full scale image.
}

interface IEnergyMix {
    is_green_energy:                boolean;                        // True if 100% from regenerative sources. (CO2 and nuclear waste is zero)
    energy_sources?:                Array<IEnergySource>;           // Key-value pairs (enum + percentage) of energy sources of this location’s tariff.
    environ_impact?:                Array<IEnvironmentalImpact>;    // Key-value pairs (enum + percentage) of nuclear waste and CO2 exhaust of this location’s tariff.
    supplier_name?:                 string;                         // Name of the energy supplier, delivering the energy for this location or tariff.*
    energy_product_name?:           string;                         // Name of the energy suppliers product/tariff plan used at this location.*
}

interface IEnergySource {
    source:                         EnergySourceCategory;           // The type of energy source.
    percentage:                     number;                         // Percentage of this source (0-100) in the mix.
}

interface IEnvironmentalImpact {
    category:                       EnvironmentalImpactCategory;    // The environmental impact category of this value.
    amount:                         number;                         // Amount of this portion in g/kWh.
}

type Capability =
    "CHARGING_PROFILE_CAPABLE"         |                            // The EVSE supports charging profiles.
    "CHARGING_PREFERENCES_CAPABLE"     |                            // The EVSE supports charging preferences.
    "CHIP_CARD_SUPPORT"                |                            // EVSE has a payment terminal that supports chip cards.
    "CONTACTLESS_CARD_SUPPORT"         |                            // EVSE has a payment terminal that supports contactless cards.
    "CREDIT_CARD_PAYABLE"              |                            // EVSE has a payment terminal that makes it possible to pay for charging using a credit card.
    "DEBIT_CARD_PAYABLE"               |                            // EVSE has a payment terminal that makes it possible to pay for charging using a debit card.
    "ISO_15118_2_PLUG_AND_CHARGE"      |                            // The EVSE supports authentication of the Driver using a contract certificate stored in the vehicle according to ISO 15118-2.
    "ISO_15118_20_PLUG_AND_CHARGE"     |                            // The EVSE supports authentication of the Driver using a contract certificate stored in the vehicle according to ISO 15118-20.
    "PED_TERMINAL"                     |                            // EVSE has a payment terminal with a pin-code entry device.
    "REMOTE_START_STOP_CAPABLE"        |                            // The EVSE can remotely be started/stopped.
    "RESERVABLE"                       |                            // The EVSE can be reserved.
    "RFID_READER"                      |                            // Charging at this EVSE can be authorized with an RFID token.
    "START_SESSION_CONNECTOR_REQUIRED" |                            // When a StartSession is sent to this EVSE, the MSP is required to add the optional connector_id field in the StartSession object.
    "TOKEN_GROUP_CAPABLE"              |                            // This EVSE supports token groups, two or more tokens work as one, so that a session can be started with one token and stopped with another (handy when a card and key-fob are given to the EV-driver).
    "UNLOCK_CAPABLE"                   |                            // Connectors have mechanical lock that can be requested by the eMSP to be unlocked.
     string;

type ConnectorFormat =
    "SOCKET" |                                                      // The connector is a socket; the EV user needs to bring a fitting plug.
    "CABLE";                                                        // The connector is an attached cable; the EV users car needs to have a fitting inlet.

type ConnectorType =
    "CHADEMO"               |                                       // The connector type is CHAdeMO, DC
    "CHAOJI"                |                                       // The ChaoJi connector. The new generation charging connector, harmonized between CHAdeMO and GB/T. DC.
    "DOMESTIC_A"            |                                       // Standard/Domestic household, type "A", NEMA 1-15, 2 pins
    "DOMESTIC_B"            |                                       // Standard/Domestic household, type "B", NEMA 5-15, 3 pins
    "DOMESTIC_C"            |                                       // Standard/Domestic household, type "C", CEE 7/17, 2 pins
    "DOMESTIC_D"            |                                       // Standard/Domestic household, type "D", 3 pin
    "DOMESTIC_E"            |                                       // Standard/Domestic household, type "E", CEE 7/5 3 pins
    "DOMESTIC_F"            |                                       // Standard/Domestic household, type "F", CEE 7/4, Schuko, 3 pins
    "DOMESTIC_G"            |                                       // Standard/Domestic household, type "G", BS 1363, Commonwealth, 3 pins
    "DOMESTIC_H"            |                                       // Standard/Domestic household, type "H", SI-32, 3 pins
    "DOMESTIC_I"            |                                       // Standard/Domestic household, type "I", AS 3112, 3 pins
    "DOMESTIC_J"            |                                       // Standard/Domestic household, type "J", SEV 1011, 3 pins
    "DOMESTIC_K"            |                                       // Standard/Domestic household, type "K", DS 60884-2-D1, 3 pins
    "DOMESTIC_L"            |                                       // Standard/Domestic household, type "L", CEI 23-16-VII, 3 pins
    "DOMESTIC_M"            |                                       // Standard/Domestic household, type "M", BS 546, 3 pins
    "DOMESTIC_N"            |                                       // Standard/Domestic household, type "N", NBR 14136, 3 pins"
    "DOMESTIC_O"            |                                       // Standard/Domestic household, type "O", TIS 166-2549, 3 pins
    "GBT_AC"                |                                       // Guobiao GB/T 20234.2 AC socket/connector
    "GBT_DC"                |                                       // Guobiao GB/T 20234.3 DC connector
    "IEC_60309_2_single_16" |                                       // IEC 60309-2 Industrial Connector single phase 16 amperes (usually blue)
    "IEC_60309_2_three_16"  |                                       // IEC 60309-2 Industrial Connector three phases 16 amperes (usually red)
    "IEC_60309_2_three_32"  |                                       // IEC 60309-2 Industrial Connector three phases 32 amperes (usually red)
    "IEC_60309_2_three_64"  |                                       // IEC 60309-2 Industrial Connector three phases 64 amperes (usually red)
    "IEC_62196_T1"          |                                       // IEC 62196 Type 1 "SAE J1772"
    "IEC_62196_T1_COMBO"    |                                       // Combo Type 1 based, DC
    "IEC_62196_T2"          |                                       // IEC 62196 Type 2 "Mennekes"
    "IEC_62196_T2_COMBO"    |                                       // Combo Type 2 based, DC
    "IEC_62196_T3A"         |                                       // IEC 62196 Type 3A
    "IEC_62196_T3C"         |                                       // IEC 62196 Type 3C "Scame"
    "MCS"                   |                                       // The MegaWatt Charging System (MCS) connector as developed by CharIN.
    "NEMA_5_20"             |                                       // NEMA 5-20, 3 pins
    "NEMA_6_30"             |                                       // NEMA 6-30, 3 pins
    "NEMA_6_50"             |                                       // NEMA 6-50, 3 pins
    "NEMA_10_30"            |                                       // NEMA 10-30, 3 pins
    "NEMA_10_50"            |                                       // NEMA 10-50, 3 pins
    "NEMA_14_30"            |                                       // NEMA 14-30, 3 pins, rating of 30 A
    "NEMA_14_50"            |                                       // NEMA 14-50, 3 pins, rating of 50 A
    "PANTOGRAPH_BOTTOM_UP"  |                                       // On-board Bottom-up-Pantograph typically for bus charging
    "PANTOGRAPH_TOP_DOWN"   |                                       // Off-board Top-down-Pantograph typically for bus charging
    "SAE_J3400"             |                                       // SAE J3400, also known as North American Charging Standard (NACS), developed by Tesla, Inc in 2021.
    "TESLA_R"               |                                       // Tesla Connector "Roadster" - type(round, 4 pin)
    "TESLA_S"               |                                       // Tesla Connector "Model-S" - type(oval, 5 pin). Mechanically compatible with SAE J3400 but uses CAN bus for communication instead of power line communication.
     string;

type EnergySourceCategory =
    "NUCLEAR"        |                                              // Nuclear power sources.
    "GENERAL_FOSSIL" |                                              // All kinds of fossil power sources.
    "COAL"           |                                              // Fossil power from coal.
    "GAS"            |                                              // Fossil power from gas.
    "GENERAL_GREEN"  |                                              // All kinds of regenerative power sources.
    "SOLAR"          |                                              // Regenerative power from PV.
    "WIND"           |                                              // Regenerative power from wind turbines.
    "WATER"          |                                              // Regenerative power from water turbines.
     string;

type EnvironmentalImpactCategory =
    "NUCLEAR_WASTE"  |                                              // Produced nuclear waste in gramms per kilowatthour.
    "CARBON_DIOXIDE" |                                              // Exhausted carbon dioxide in gramms per kilowarrhour.
     string;

type Facility =
    "HOTEL"           |                                             // A hotel.
    "RESTAURANT"      |                                             // A restaurant.
    "CAFE"            |                                             // A cafe.
    "MALL"            |                                             // A mall or shopping center.
    "SUPERMARKET"     |                                             // A supermarket.
    "SPORT"           |                                             // Sport facilities: gym, field etc.
    "RECREATION_AREA" |                                             // A Recreation area.
    "NATURE"          |                                             // Located in, or close to, a park, nature reserve/park etc.
    "MUSEUM"          |                                             // A museum.
    "BIKE_SHARING"    |                                             // A bike/e-bike/e-scooter sharing location.
    "BUS_STOP"        |                                             // A bus stop.
    "TAXI_STAND"      |                                             // A taxi stand.
    "TRAM_STOP"       |                                             // A tram stop/station.
    "METRO_STATION"   |                                             // A metro station.
    "TRAIN_STATION"   |                                             // A train station.
    "AIRPORT"         |                                             // An airport.
    "PARKING_LOT"     |                                             // A parking lot.
    "CARPOOL_PARKING" |                                             // A carpool parking.
    "FUEL_STATION"    |                                             // A Fuel station.
    "WIFI"            |                                             // Wifi or other type of internet available.
     string;

type ImageCategory =
    "CHARGER"  |                                                    // Photo of the physical device that contains one or more EVSEs.
    "ENTRANCE" |                                                    // Location entrance photo.Should show the car entrance to the location from street side.
    "LOCATION" |                                                    // Location overview photo.
    "NETWORK"  |                                                    // logo of an associated roaming network to be displayed with the EVSE for example in lists, maps and detailed information view
    "OPERATOR" |                                                    // logo of the charge points operator, for example a municipality, to be displayed with the EVSEs detailed information view or in lists and maps, if no networkLogo is present
    "OTHER"    |                                                    // Other
    "OWNER"    |                                                    // logo of the charge points owner, for example a local store, to be displayed with the EVSEs detailed information view
     string;

type ParkingType =
    "ALONG_MOTORWAY"     |                                          // Location on a parking facility/rest area along a motorway, freeway, interstate, highway etc.
    "PARKING_GARAGE"     |                                          // Multistorey car park.
    "PARKING_LOT"        |                                          // A cleared area that is intended for parking vehicles, i.e.at super markets, bars, etc.
    "ON_DRIVEWAY"        |                                          // Location is on the driveway of a house/building.
    "ON_STREET"          |                                          // Parking in public space.
    "UNDERGROUND_GARAGE" |                                          // Multistorey car park, mainly underground.
     string;

type ParkingRestriction =
    "EV_ONLY"     |                                                 // Reserved parking spot for electric vehicles.
    "PLUGGED"     |                                                 // Parking is only allowed while plugged in (charging).
    "CUSTOMERS"   |                                                 // Parking spot for customers/guests only, for example in case of a hotel or shop.
    "TAXI"        |                                                 // Parking only for taxi vehicles.
    "TENANTS"     |                                                 // Parking only for people who live in a complex that the Location belongs to.
     string;

type PowerType =
    "AC_1_PHASE"       |                                            // AC single phase
    "AC_2_PHASE"       |                                            // AC two phases, only two of the three available phases connected
    "AC_2_PHASE_SPLIT" |                                            // AC two phases using split phase system
    "AC_3_PHASE"       |                                            // AC three phases
    "DC";                                                           // Direct Current

type Status  =
    "AVAILABLE"   |                                                 // The EVSE/Connector is able to start a new charging session.
    "BLOCKED"     |                                                 // The EVSE/Connector is not accessible because of a physical barrier, i.e.a car.
    "CHARGING"    |                                                 // The EVSE/Connector is in use.
    "INOPERATIVE" |                                                 // The EVSE/Connector is not yet active, or temporarily not available for use, but not broken or defect.
    "OUTOFORDER"  |                                                 // The EVSE/Connector is currently out of order, some part/components may be broken/defect.
    "PLANNED"     |                                                 // The EVSE/Connector is planned, will be operating soon.
    "REMOVED"     |                                                 // The EVSE/Connector was discontinued/removed.
    "RESERVED"    |                                                 // The EVSE/Connector is reserved for a particular EV driver and is unavailable for other drivers.
    "UNKNOWN"     |                                                 // No status information available (also used when offline).
     string;

interface IEVSE {
    uid:                            string;                         // Uniquely identifies the EVSE within the CPOs platform (and suboperator platforms). This field can never be changed, modified or renamed. This is the 'technical' identification of the EVSE, not to be used as 'human readable' identification, use the field evse_id for that. This field is named uid instead of id, because id could be confused with evse_id which is an eMI3 defined field. Note that in order to fulfill both the requirement that an EVSE’s uid be unique within a CPO’s platform and the requirement that EVSEs are never deleted, a CPO will typically want to avoid using identifiers of the physical hardware for this uid property. If they do use such a physical identifier, they will find themselves breaking the uniqueness requirement for uid when the same physical EVSE is redeployed at another Location.
    evse_id?:                       string;                         // Compliant with the following specification for EVSE ID from "eMI3 standard version V1.0" (https://web.archive.org/web/20230603153631/https://emi3group.com/documents-links/) "Part 2: business objects." Optional because: if an evse_id is to be re-used in the real world, the evse_id can be removed from an EVSE object if the status is set to REMOVED.
    status:                         Status;                         // Indicates the current status of the EVSE.
    status_schedule?:               Array<IStatusSchedule>;         // Indicates a planned status update of the EVSE.
    capabilities?:                  Array<Capability>;              // List of functionalities that the EVSE is capable of.
    connectors:                     Array<IConnector>;              // List of available connectors on the EVSE.
    energy_meter?:                  IEnergyMeter;                   // Optional energy meter [OCPI Computer Science Extension!]
    floor_level?:                   string;                         // Level on which the Charge Point is located (in garage buildings) in the locally displayed numbering scheme.
    coordinates?:                   IGeoLocation;                   // Coordinates of the EVSE.
    physical_reference?:            string;                         // A number/string printed on the outside of the EVSE for visual identification.
    directions?:                    Array<IDisplayText>;            // Multi-language human-readable directions when more detailed information on how to reach the EVSE from the Location is required.
    parking_restrictions?:          Array<ParkingRestriction>;      // The restrictions that apply to the parking spot.
    images?:                        Array<IImage>;                  // Links to images related to the EVSE such as photos or logos.
    created?:                       string;                         // Optional timestamp when this EVSE was created [OCPI Computer Science Extension!]
    last_updated:                   string;                         // Timestamp when this EVSE or one of its connectors was last updated (or created).
}

interface IStatusSchedule {
    period_begin:                   string;                         // Begin of the scheduled period.
    period_end?:                    string;                         // End of the scheduled period, if known.
    status:                         Status;                         // Status value during the scheduled period.
}

interface IConnector {
    id:                             string;                         // Identifier of the Connector within the EVSE. Two Connectors may have the same id as long as they do not belong to the same EVSE object.
    standard:                       ConnectorType;                  // The standard of the installed connector.
    format:                         ConnectorFormat;                // The format (socket/cable) of the installed connector.
    power_type:                     PowerType;                      // The power type of the connector.
    max_voltage:                    number;                         // Maximum voltage of the connector (line to neutral for AC_3_PHASE), in volt [V]. For example: DC Chargers might vary the voltage during charging when battery almost full.
    max_amperage:                   number;                         // Maximum amperage of the connector, in ampere [A].
    max_electric_power?:            number;                         // Maximum electric power that can be delivered by this connector, in Watts (W). When the maximum electric power is lower than the calculated value from voltage and amperage, this value should be set. For example: A DC Charge Point which can delivers up to 920V and up to 400A can be limited to a maximum of 150kW (max_electric_power = 150000). Depending on the car, it may supply max voltage or current, but not both at the same time. For AC Charge Points, the amount of phases used can also have influence on the maximum power.
    tariff_ids?:                    Array<string>;                  // Identifiers of the currently valid charging tariffs. Multiple tariffs are possible, but only one of each Tariff.type can be active at the same time. Tariffs with the same type are only allowed if they are not active at the same time: start_date_time and end_date_time period not overlapping. When preference-based smart charging is supported, one tariff for every possible ProfileType should be provided. These tell the user about the options they have at this Connector, and what the tariff is for every option. For a "free of charge" tariff, this field should be set and point to a defined "free of charge" tariff.
    terms_and_conditions?:          string;                         // URL to the operator’s terms and conditions.
    created?:                       string;                         // Optional timestamp when this connector was created [OCPI Computer Science Extension!]
    last_updated:                   string;                         // Timestamp when this connector was last updated (or created).
}

type TariffType =
    "AD_HOC_PAYMENT" |                                              // Used to describe that a Tariff is valid when ad-hoc payment is used at the Charge Point (for example: Debit or Credit card payment terminal).
    "PROFILE_CHEAP"  |                                              // Used to describe that a Tariff is valid when Charging Preference: CHEAP is set for the session.
    "PROFILE_FAST"   |                                              // Used to describe that a Tariff is valid when Charging Preference: FAST is set for the session.
    "PROFILE_GREEN"  |                                              // Used to describe that a Tariff is valid when Charging Preference: GREEN is set for the session.
    "REGULAR"        |                                              // Used to describe that a Tariff is valid when using an RFID, without any Charging Preference, or when Charging Preference: REGULAR is set for the session.
     string;


type TaxIncluded =
    "YES" |                                                         // Taxes are included in the prices in this Tariff.
    "NO"  |                                                         // Taxes are not included, and will be added on top of the prices in this Tariff.
    "N/A";                                                          // No taxes are applicable to this Tariff.

interface ITaxAmount {
    name:                           string,                         // A description of the tax. In countries where a tax name is required like Canada this can be something like "QST". In countries where this is not required, this can be something more generic like "VAT" or "General Sales Tax".
    account_number?:                string,                         // Tax Account Number of the business entity remitting these taxes. Optional as this is not required in all countries.
    percentage?:                    number,                         // Tax percentage. Optional as this is not required in all countries.
    amount:                         number                          // The amount of money of this tax that is due.
}

interface IPrice {
    excl_vat:                       number;                         // Price/Cost excluding VAT.
    incl_vat?:                      number;                         // Price/Cost including VAT.
}

interface ITariff {
    country_code:                   string,                         // ISO-3166 alpha-2 country code of the CPO that owns this Tariff.
    party_id:                       string,                         // ID of the CPO that 'owns' this Tariff (following the ISO-15118 standard).
    id:                             string,                         // Uniquely identifies the tariff within the CPO’s platform (and suboperator platforms).
    currency:                       string,                         // ISO-4217 code of the currency of this tariff.
    type?:                          TariffType,                     // Defines the type of the tariff. This allows for distinction in case of given Charging Preferences. When omitted, this tariff is valid for all sessions.
    tariff_alt_text?:               Array<IDisplayText>,            // Optional list of multi-language alternative tariff info texts.
    tariff_alt_url?:                string,                         // URL to a web page that contains an explanation of the tariff information in human readable form.
    min_price?:                     IPrice,                         // When this field is set, a Charging Session with this tariff will at least cost this amount. This is different from a `FLAT` fee (Start Tariff, Transaction Fee), as a `FLAT` fee is a fixed amount that has to be paid for any Charging Session. A minimum price indicates that when the cost of a Charging Session is lower than this amount, the cost of the Session will be equal to this amount. (Also see note below)
    max_price?:                     IPrice,                         // When this field is set, a Charging Session with this tariff will NOT cost more than this amount. (See note below)
    elements:                       Array<ITariffElement>,          // List of tariff elements
    tax_included:                   TaxIncluded,                    // Whether taxes are included in the amounts in this Tariff.
    start_date_time?:               string,                         // The time when this tariff becomes active, in UTC, time_zone field of the Location can be used to convert to local time. Typically used for a new tariff that is already given with the location, before it becomes active. (See note below)
    end_date_time?:                 string,                         // The time after which this tariff is no longer valid, in UTC, time_zone field if the Location can be used to convert to local time. Typically used when this tariff is going to be replaced with a different tariff in the near future. (See note below)
    energy_mix?:                    IEnergyMix,                     // Details on the energy supplied with this tariff.
    created?:                       string;                         // Optional timestamp when this Tariff was created [OCPI Computer Science Extension!]
    last_updated:                   string                          // Timestamp when this tariff was last updated (or created).
}

interface ITariffMetadata extends TMetadataDefaults {

}

interface ITariffElement {
    price_components:               Array<IPriceComponent>,         // List of price components that make up the pricing of this tariff
    restrictions?:                  ITariffRestrictions             // Tariff restrictions object
}

type TariffDimension =
    "ENERGY"       |                                                // Defined in kWh, step_size multiplier: 1 Wh
    "FLAT"         |                                                // Flat fee without unit for step_size
    "PARKING_TIME" |                                                // Time not charging: defined in hours, step_size multiplier: 1 second
    "TIME";                                                         // Time charging: defined in hours, step_size multiplier: 1 second. Can also be used in combination with a RESERVATION restriction to describe the price of the reservation time.

interface IPriceComponent {
    type:                           TariffDimension,                // The dimension that is being priced.
    price:                          number,                         // Price per unit (excl. VAT) for this dimension.
    vat?:                           number,                         // Applicable VAT percentage for this tariff dimension. If omitted, no VAT is applicable.
    step_size:                      number                          // Minimum amount to be billed. That is, the dimension will be billed in this step_size blocks. Consumed amounts are rounded up to the smallest multiple of step_size that is greater than the consumed amount. For example: if type is TIME and step_size has a value of 300, then time will be billed in blocks of 5 minutes. If 6 minutes were consumed, 10 minutes (2 blocks of step_size) will be billed.
}

type DayOfWeek =
    "MONDAY"    |
    "TUESDAY"   |
    "WEDNESDAY" |
    "THURSDAY"  |
    "FRIDAY"    |
    "SATURDAY"  |
    "SUNDAY";

type ReservationRestriction =
    "RESERVATION" |                                                 // Used in Tariff Elements to describe costs for a reservation.
    "RESERVATION_EXPIRES";                                          // Used in Tariff Elements to describe costs for a reservation that expires (i.e. driver does not start a charging session before expiry_date of the reservation).

interface ITariffRestrictions {
    start_time?:                    string,                         // Start time of day in local time, the time zone is defined in the time_zone field of the Location, for example 13:30, valid from this time of the day. Must be in 24h format with leading zeros. Hour/Minute separator: ":" Regex: ([0-1][0-9]|2[0-3]):[0-5][0-9]
    end_time?:                      string,                         // End time of day in local time, the time zone is defined in the time_zone field of the Location, for example 19:45, valid until this time of the day. Same syntax as start_time. If end_time < start_time then the period wraps around to the next day. To stop at end of the day use: 00:00.
    start_date?:                    string,                         // Start date in local time, the time zone is defined in the time_zone field of the Location, for example: 2015-12-24, valid from this day (inclusive). Regex: ([12][0-9]{3})-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])
    end_date?:                      string,                         // End date in local time, the time zone is defined in the time_zone field of the Location, for example: 2015-12-27, valid until this day (exclusive). Same syntax as start_date.
    min_kwh?:                       number,                         // Minimum consumed energy in kWh, for example 20, valid from this amount of energy (inclusive) being used.
    max_kwh?:                       number,                         // Maximum consumed energy in kWh, for example 50, valid until this amount of energy (exclusive) being used.
    min_current?:                   number,                         // Sum of the minimum current (in Amperes) over all phases, for example 5. When the EV is charging with more than, or equal to, the defined amount of current, this TariffElement is/becomes active. If the charging current is or becomes lower, this TariffElement is not or no longer valid and becomes inactive. This describes NOT the minimum current over the entire Charging Session. This restriction can make a TariffElement become active when the charging current is above the defined value, but the TariffElement MUST no longer be active when the charging current drops below the defined value.
    max_current?:                   number,                         // Sum of the maximum current (in Amperes) over all phases, for example 20. When the EV is charging with less than the defined amount of current, this TariffElement becomes/is active. If the charging current is or becomes higher, this TariffElement is not or no longer valid and becomes inactive. This describes NOT the maximum current over the entire Charging Session. This restriction can make a TariffElement become active when the charging current is below this value, but the TariffElement MUST no longer be active when the charging current raises above the defined value.
    min_power?:                     number,                         // Minimum power in kW, for example 5. When the EV is charging with more than, or equal to, the defined amount of power, this TariffElement is/becomes active. If the charging power is or becomes lower, this TariffElement is not or no longer valid and becomes inactive. This describes NOT the minimum power over the entire Charging Session. This restriction can make a TariffElement become active when the charging power is above this value, but the TariffElement MUST no longer be active when the charging power drops below the defined value.
    max_power?:                     number,                         // Maximum power in kW, for example 20. When the EV is charging with less than the defined amount of power, this TariffElement becomes/is active. If the charging power is or becomes higher, this TariffElement is not or no longer valid and becomes inactive. This describes NOT the maximum power over the entire Charging Session. This restriction can make a TariffElement become active when the charging power is below this value, but the TariffElement MUST no longer be active when the charging power raises above the defined value.
    min_duration?:                  number,                         // Minimum duration in seconds the Charging Session MUST last (inclusive). When the duration of a Charging Session is longer than the defined value, this TariffElement is or becomes active. Before that moment, this TariffElement is not yet active.
    max_duration?:                  number,                         // Maximum duration in seconds the Charging Session MUST last (exclusive). When the duration of a Charging Session is shorter than the defined value, this TariffElement is or becomes active. After that moment, this TariffElement is no longer active.
    day_of_week?:                   Array<DayOfWeek>,               // Which day(s) of the week this TariffElement is active.
    reservation?:                   ReservationRestriction          // When this field is present, the TariffElement describes reservation costs. A reservation starts when the reservation is made, and ends when the driver starts charging on the reserved EVSE/Location, or when the reservation expires. A reservation can only have: FLAT and TIME TariffDimensions, where TIME is for the duration of the reservation.
}


type TokenType =
    "AD_HOC_USER" |                                                 // One time use Token ID generated by a server (or App.) The eMSP uses this to bind a Session to a customer, probably an app user.
    "APP_USER"    |                                                 // Token ID generated by a server (or App.) to identify a user of an App. The same user uses the same Token for every Session.
    "EMAID"       |                                                 // An EMAID. EMAIDs are used as Tokens when the Charging Station and the vehicle are using ISO 15118 for communication.
    "OTHER"       |                                                 // Other type of token
    "RFID"        |                                                 // RFID Token
     string;

type Allowed =
    "ALLOWED"     |                                                 // This Token is allowed to charge at this location.
    "BLOCKED"     |                                                 // This Token is blocked.
    "EXPIRED"     |                                                 // This Token has expired.
    "NO_CREDIT"   |                                                 // This Token belongs to an account that has not enough credits to charge at the given location.
    "NOT_ALLOWED" |                                                 // Token is valid, but is not allowed to charge at the given location.
     string;

type WhitelistType =
    "ALWAYS"          |                                             // Token always has to be whitelisted, realtime authorization is not possible/allowed. CPO shall always allow any use of this Token.
    "ALLOWED"         |                                             // It is allowed to whitelist the token, realtime authorization is also allowed. The CPO may choose which version of authorization to use.
    "ALLOWED_OFFLINE" |                                             // In normal situations realtime authorization shall be used. But when the CPO cannot get a response from the eMSP (communication between CPO and eMSP is offline), the CPO shall allow this Token to be used.
    "NEVER";                                                        // Whitelisting is forbidden, only realtime authorization is allowed. CPO shall always send a realtime authorization for any use of this Token to the eMSP.

type ProfileType =
    "CHEAP"   |                                                     // Driver wants to use the cheapest charging profile possible.
    "FAST"    |                                                     // Driver wants his EV charged as quickly as possible and is willing to pay a premium for this, if needed.
    "GREEN"   |                                                     // Driver wants his EV charged with as much regenerative (green) energy as possible.
    "REGULAR" |                                                     // Driver does not have special preferences.
     string;

interface IEnergyContract {
    supplier_name:                  string;                         // Name of the energy supplier for this token.
    contract_id?:                   string;                         // Contract ID at the energy supplier, that belongs to the owner of this token.
}

interface IPublishToken {
    uid?:                           string;                         // Unique ID by which this Token can be identified.
    type?:                          TokenType;                      // Type of the token
    visual_number?:                 string;                         // Visual readable number/identification as printed on the Token (RFID card).
    issuer?:                        string;                         // Issuing company, most of the times the name of the company printed on the token (RFID card), not necessarily the eMSP.
    group_id?:                      string;                         // This ID groups a couple of tokens. This can be used to make two or more tokens work as one.
}

interface IToken {
    country_code:                   string,                         // ISO-3166 alpha-2 country code of the MSP that 'owns' this Token.
    party_id:                       string,                         // ID of the eMSP that 'owns' this Token (following the ISO-15118 standard).
    uid:                            string;                         // Unique ID by which this Token, combined with the Token type, can be identified. This is the field used by CPO system (RFID reader on the Charge Point) to identify this token. Currently, in most cases: type=RFID, this is the RFID hidden ID as read by the RFID reader, but that is not a requirement. If this is a APP_USER or AD_HOC_USER Token, it will be a uniquely, by the eMSP, generated ID. This field is named uid instead of id to prevent confusion with: contract_id.
    type:                           TokenType;                      // Type of the token
    contract_id:                    string;                         // Uniquely identifies the EV Driver contract token within the eMSP’s platform (and suboperator platforms). Recommended to follow the specification for eMA ID from "eMI3 standard version V1.0" (http://emi3group.com/documents-links/) "Part 2: business objects."
    visual_number?:                 string;                         // Visual readable number/identification as printed on the Token (RFID card), might be equal to the contract_id.
    issuer:                         string;                         // Issuing company, most of the times the name of the company printed on the token (RFID card), not necessarily the eMSP.
    group_id?:                      string;                         // This ID groups a couple of tokens. This can be used to make two or more tokens work as one, so that a session can be started with one token and stopped with another, handy when a card and key-fob are given to the EV-driver. Beware that OCPP 1.5/1.6 only support group_ids (it is called parentId in OCPP 1.5/1.6) with a maximum length of 20.
    valid:                          boolean;                        // Is this Token valid?
    whitelist:                      WhitelistType;                  // Indicates what type of white-listing is allowed.
    language?:                      string;                         // Language Code ISO 639-1. This optional field indicates the Token owner’s preferred interface language. If the language is not provided or not supported then the CPO is free to choose its own language.
    default_profile_type?:          ProfileType;                    // The default Charging Preference. When this is provided, and a charging session is started on an Charge Point that support Preference base Smart Charging and support this ProfileType, the Charge Point can start using this ProfileType, without this having to be set via: Set Charging Preferences.
    energy_contract?:               IEnergyContract;                // When the Charge Point supports using your own energy supplier/contract at a Charge Point, information about the energy supplier/contract is needed so the CPO knows which energy supplier to use. NOTE: In a lot of countries it is currently not allowed/possible to use a drivers own energy supplier/contract at a Charge Point.
    created?:                       string;                         // Optional timestamp when this token was created [OCPI Computer Science Extension!]
    last_updated:                   string;                         // Timestamp when this Token was last updated (or created).
}

interface ITokenMetadata extends TMetadataDefaults {

}


type AuthMethod =
    "AUTH_REQUEST" |                                                // Authentication request from the eMSP
    "WHITELIST"    |                                                // Whitelist used to authenticate, no request done to the eMSP
     string;

type CdrDimensionType =
    "ENERGY"       |                                                // defined in kWh, default step_size is 1 Wh
    "FLAT"         |                                                // flat fee, no unit
    "MAX_CURRENT"  |                                                // defined in A (Ampere), Maximum current reached during charging session
    "MIN_CURRENT"  |                                                // defined in A (Ampere), Minimum current used during charging session
    "PARKING_TIME" |                                                // time not charging: defined in hours, default step_size is 1 second
    "TIME"         |                                                // time charging: defined in hours, default step_size is 1 second
     string;

type SessionStatus =
    "ACTIVE"      |                                                 // The session has been accepted and is active. All pre-conditions were met: Communication between EV and EVSE (for example: cable plugged in correctly), EV or driver is authorized. EV is being charged, or can be charged. Energy is, or is not, being transfered.
    "COMPLETED"   |                                                 // The session has been finished successfully. No more modifications will be made to the Session object using this state.
    "INVALID"     |                                                 // The Session object using this state is declared invalid and will not be billed.
    "PENDING"     |                                                 // The session is pending, it has not yet started. Not all pre-conditions are met. This is the initial state. The session might never become an active session.
    "RESERVATION" |                                                 // The session is started due to a reservation, charging has not yet started. The session might never become an active session.
     string;

interface ICDRDimension {
    type:                           CdrDimensionType;               // Type of cdr dimension
    volume:                         number;                         // Volume of the dimension consumed, measured according to the dimension type.
}

interface IChargingPeriod {
    start_date_time:                string;                         // Start date and time of this charging period.
    dimensions:                     Array<ICDRDimension>;           // List of dimensions for this charging period.
    tariff_id?:                     string;                         // Unique identifier of the Tariff that is relevant for this Charging Period. If not provided, no Tariff is relevant during this period.
}

interface ISession {
    country_code:                   string,                         // ISO-3166 alpha-2 country code of the CPO that 'owns' this Session.
    party_id:                       string,                         // ID of the CPO that 'owns' this Session (following the ISO-15118 standard).
    id:                             string;                         // The unique id that identifies the charging session in the CPO platform.
    start_datetime:                 string;                         // The timestamp when the session became ACTIVE in the Charge Point. When the session is still PENDING, this field SHALL be set to the time the Session was created at the Charge Point. When a Session goes from PENDING to ACTIVE, this field SHALL be updated to the moment the Session went to ACTIVE in the Charge Point.
    end_datetime?:                  string;                         // The timestamp when the session was completed/finished, charging might have finished before the session ends, for example: EV is full, but parking cost also has to be paid.
    kwh:                            number;                         // How many kWh were charged.
    cdr_token:                      ICDRToken;                      // Token used to start this charging session, including all the relevant information to identify the unique token.
    auth_method:                    AuthMethod;                     // Method used for authentication. This might change during a session, for example when the session was started with a reservation: ReserveNow: COMMAND. When the driver arrives and starts charging using a Token that is whitelisted: WHITELIST.
    authorization_reference?:       string;                         // Reference to the authorization given by the eMSP. When the eMSP provided an authorization_reference in either: real-time authorization, StartSession or ReserveNow this field SHALL contain the same value. When different authorization_reference values have been given by the eMSP that are relevant to this Session, the last given value SHALL be used here.
    location_id:                    string;                         // Location.id of the Location object of this CPO, on which the charging session is/was happening.
    evse_uid:                       string;                         // EVSE.uid of the EVSE of this Location on which the charging session is/was happening. Allowed to be set to: #NA when this session is created for a reservation, but no EVSE yet assigned to the driver.
    connector_id:                   string;                         // Connector.id of the Connector of this Location where the charging session is/was happening. Allowed to be set to: #NA when this session is created for a reservation, but no connector yet assigned to the driver.
    meter_id?:                      string;                         // Optional identification of the kWh meter.
    currency:                       string;                         // ISO 4217 code of the currency used for this session.
    charging_periods?:              Array<IChargingPeriod>;         // An optional list of Charging Periods that can be used to calculate and verify the total cost.
    total_cost?:                    IPrice;                         // The total cost of the session in the specified currency. This is the price that the eMSP will have to pay to the CPO. A total_cost of 0.00 means free of charge. When omitted, i.e. no price information is given in the Session object, it does not imply the session is/was free of charge.
    status:                         SessionStatus;                  // The status of the session.
    created?:                       string;                         // Optional timestamp when this session was created [OCPI Computer Science Extension!]
    last_updated:                   string;                         // Timestamp when this session was last updated (or created).
}

interface ISessionMetadata extends TMetadataDefaults {

}

interface ICDRToken {
    country_code:                   string;                         // ISO-3166 alpha-2 country code of the MSP that 'owns' this Token.
    party_id:                       string;                         // ID of the eMSP that 'owns' this Token (following the ISO-15118 standard).
    uid:                            string;                         // Unique ID by which this Token can be identified. This is the field used by the CPO’s system (RFID reader on the Charge Point) to identify this token. Currently, in most cases: type=RFID, this is the RFID hidden ID as read by the RFID reader, but that is not a requirement. If this is a type=APP_USER Token, it will be a unique, by the eMSP, generated ID.
    type:                           TokenType;                      // Type of the token.
    contract_id:                    string;                         // Uniquely identifies the EV driver contract token within the eMSP’s platform (and suboperator platforms). Recommended to follow the specification for eMA ID from "eMI3 standard version V1.0" (https://web.archive.org/web/20230603153631/https://emi3group.com/documents-links/) "Part 2: business objects."
}

interface ICDRLocation {
    id:                             string;                         // Uniquely identifies the location within the CPO’s platform (and suboperator platforms). This field can never be changed, modified or renamed.
    name?:                          string;                         // Display name of the location.
    address:                        string;                         // Street/block name and house number if available.
    city:                           string;                         // City or town.
    postal_code?:                   string;                         // Postal code of the location, may only be omitted when the location has no postal code: in some countries charging locations at highways don’t have postal codes.
    state?:                         string;                         // State only to be used when relevant.
    country:                        string;                         // ISO 3166-1 alpha-2 code for the country of this location.
    coordinates:                    IGeoLocation;                   // Coordinates of the location.
    evse_uid:                       string;                         // Uniquely identifies the EVSE within the CPO’s platform (and suboperator platforms). For example a database unique ID or the actual EVSE ID. This field can never be changed, modified or renamed. This is the technical identification of the EVSE, not to be used as human readable identification, use the field: evse_id for that. Allowed to be set to: #NA when this CDR is created for a reservation that never resulted in a charging session.
    evse_id:                        string;                         // Human readable identification of the EVSE. This is the identification that is shown to the EV driver. Allowed to be set to: #NA when this CDR is created for a reservation that never resulted in a charging session.
    connector_id:                   string;                         // Identifier of the connector within the EVSE. Allowed to be set to: #NA when this CDR is created for a reservation that never resulted in a charging session.
    connector_standard:             ConnectorType;                  // The standard of the installed connector. When this CDR is created for a reservation that never resulted in a charging session, this field can be set to any value and should be ignored by the Receiver.
    connector_format:               ConnectorFormat;                // The format (socket/cable) of the installed connector. When this CDR is created for a reservation that never resulted in a charging session, this field can be set to any value and should be ignored by the Receiver.
    connector_power_type:           PowerType;                      // The power type of the installed connector. When this CDR is created for a reservation that never resulted in a charging session, this field can be set to any value and should be ignored by the Receiver.
}

interface ISignedValue {
    nature:                         string;                         // Nature of the value, in other words, the event this value belongs to. Possible values at moment of writing: - Start (value at the start of the Session) - End (signed value at the end of the Session) - Intermediate (signed values take during the Session, after Start, before End) Others might be added later.
    plain_data:                     string;                         // The un-encoded string of data. The format of the content depends on the EncodingMethod field.
    signed_data:                    string;                         // Blob of signed data, base64 encoded. The format of the content depends on the EncodingMethod field.
}

type SignedDataEncodingMethod =
    "OCMF"                       |                                  // Open Charge Point Protocol (OCPP) Message Format
    "Alfen Eichrecht"            |                                  // Alfen Eichrecht encoding / implementation
    "EDL40 E-Mobility Extension" |                                  // eBee smart technologies implementation
    "EDL40 Mennekes"             |                                  // Mennekes implementation
     string;

interface ISignedData {
    encoding_method:                SignedDataEncodingMethod;       // The name of the encoding used in the SignedData field. This is the name given to the encoding by a company or group of companies. See note below.
    encoding_method_version?:       number;                         // Version of the EncodingMethod (when applicable)
    public_key?:                    string;                         // Public key used to sign the data, base64 encoded.
    signed_values:                  Array<ISignedValue>;            // One or more signed values.
    url?:                           string;                         // URL that can be shown to an EV driver. This URL gives the EV driver the possibility to check the signed data from a charging session.
}

interface ICDR {
    country_code:                   string,                         // ISO-3166 alpha-2 country code of the CPO that 'owns' this CDR.
    party_id:                       string,                         // ID of the CPO that 'owns' this CDR (following the ISO-15118 standard).
    id:                             string;                         // Uniquely identifies the CDR, the ID SHALL be unique per country_code/party_id combination. This field is longer than the usual 36 characters to allow for credit CDRs to have something appended to the original ID. Normal (non-credit) CDRs SHALL only have an ID with a maximum length of 36.
    start_date_time:                string;                         // Start timestamp of the charging session, or in-case of a reservation (before the start of a session) the start of the reservation.
    end_date_time:                  string;                         // The timestamp when the session was completed/finished, charging might have finished before the session ends, for example: EV is full, but parking cost also has to be paid.
    session_id?:                    string;                         // Unique ID of the Session for which this CDR is sent. Is only allowed to be omitted when the CPO has not implemented the Sessions module or this CDR is the result of a reservation that never became a charging session, thus no OCPI Session.
    cdr_token:                      ICDRToken;                      // Token used to start this charging session, including all the relevant information to identify the unique token.
    auth_method:                    AuthMethod;                     // Method used for authentication.
    authorization_reference?:       string;                         // Reference to the authorization given by the eMSP. When the eMSP provided an authorization_reference in either: real-time authorization, StartSession or ReserveNow, this field SHALL contain the same value. When different authorization_reference values have been given by the eMSP that are relevant to this Session, the last given value SHALL be used here.
    cdr_location:                   ICDRLocation;                   // Location where the charging session took place, including only the relevant EVSE and Connector.
    meter_id?:                      string;                         // Identification of the Meter inside the Charge Point.
    currency:                       string;                         // Currency of the CDR in ISO 4217 Code.
    tariffs?:                       Array<ITariff>;                 // List of relevant Tariffs, see: Tariff. When relevant, a Free of Charge tariff should also be in this list, and point to a defined Free of Charge Tariff.
    charging_periods:               Array<IChargingPeriod>;         // List of Charging Periods that make up this charging session.
    signed_data?:                   ISignedData;                    // Signed data that belongs to this charging Session.
    total_cost:                     IPrice;                         // Total sum of all the costs of this transaction in the specified currency.
    total_fixed_cost?:              IPrice;                         // Total sum of all the fixed costs in the specified currency, except fixed price components of parking and reservation. The cost not depending on amount of time/energy used etc. Can contain costs like a start tariff.
    total_energy:                   number;                         // Total energy charged, in kWh.
    total_energy_cost?:             IPrice;                         // Total sum of all the cost of all the energy used, in the specified currency.
    total_time:                     number;                         // Total duration of the charging session (including the duration of charging and not charging), in hours.
    total_time_cost?:               IPrice;                         // Total sum of all the cost related to duration of charging during this transaction, in the specified currency.
    total_parking_time?:            number;                         // Total duration of the charging session where the EV was not charging (no energy was transferred between EVSE and EV), in hours.
    total_parking_cost?:            IPrice;                         // Total sum of all the cost related to parking of this transaction, including fixed price components, in the specified currency.
    total_reservation_cost?:        IPrice;                         // Total sum of all the cost related to a reservation of a Charge Point, including fixed price components, in the specified currency.
    remark?:                        string;                         // Optional remark, can be used to provide additional human readable information to the CDR, for example: reason why a transaction was stopped.
    invoice_reference_id?:          string;                         // This field can be used to reference an invoice, that will later be send for this CDR. Making it easier to link a CDR to a given invoice. Maybe even group CDRs that will be on the same invoice.
    credit?:                        boolean;                        // When set to true, this is a Credit CDR, and the field credit_reference_id needs to be set as well.
    credit_reference_id?:           string;                         // Is required to be set for a Credit CDR. This SHALL contain the id of the CDR for which this is a Credit CDR.
    home_charging_compensation?:    boolean;                        // When set to true, this CDR is for a charging session using the home charger of the EV Driver for which the energy cost needs to be financial compensated to the EV Driver.
    created?:                       string;                         // Optional timestamp when this CDR was created [OCPI Computer Science Extension!]
    last_updated:                   string;                         // Timestamp when this CDR was last updated (or created).
}

interface ICDRMetadata extends TMetadataDefaults {

}



// -----------------------------
// OCPI Management Extensions!
// -----------------------------

type Role =
    "CPO"   |                                                       // Charge Point Operator Role.
    "EMSP"  |                                                       // eMobility Service Provider Role.
    "NAP"   |                                                       // National Access Point Role (national Database with all Location information of a country).
    "NSP"   |                                                       // Navigation Service Provider Role, role like an eMSP (probably only interested in Location information).
    "OTHER" |                                                       // Other role.
    "SCSP"  |                                                       // Smart Charging Service Provider Role.
     string;

interface IRemoteParty {
    //id:                              string;
    //"@context":                      string;
    countryCode:                     string;
    partyId:                         string;
    role:                            Role;
    businessDetails:                 IBusinessDetails;
    partyStatus:                     string;
    localAccessInfos:                Array<ILocalAccessInfo>;
    remoteAccessInfos:               Array<IRemoteAccessInfo>;
    created:                         string;
    last_updated:                    string;
}

interface IRemotePartyMetadata extends TMetadataDefaults {

}

interface IAccessInfo {
    token:                           string;
    status:                          string;
}

interface ILocalAccessInfo {
    accessToken:                     string;
    status:                          string;
    notBefore?:                      string;
    notAfter?:                       string;
    accessTokenIsBase64Encoded:      boolean;
    allowDowngrades:                 boolean;
}

interface IRemoteAccessInfo {
    accessToken:                     string;
    versionsURL:                     string;
    versionIds:                      Array<string>;
    selectedVersionId:               string;
    status:                          string;
    notBefore?:                      string;
    notAfter?:                       string;
    accessTokenIsBase64Encoded:      boolean;
    allowDowngrades:                 boolean;
}




// -----------------------------------
// OCPI Computer Science Extensions!
// -----------------------------------
interface IEnergyMeter {
    id:                              string;
    model?:                          string;
    model_url?:                      string;
    hardware_version?:               string;
    firmware_version?:               string;
    manufacturer?:                   string;
    manufacturer_url?:               string;
    public_keys?:                    Array<string>;
    public_key_certificate_chain?:   string;
    transparency_softwares?:         Array<ITransparencySoftwareStatus>;

}

// OCPI Computer Science Extension!
interface ITransparencySoftwareStatus {
    transparency_software:           ITransparencySoftware;
    legal_status:                    string;
    certificate:                     string;
    certificate_issuer:              string;
    not_before:                      string;
    not_after:                       string;
}

// OCPI Computer Science Extension!
interface ITransparencySoftware {
    name:                            string;
    version:                         string;
    open_source_license:             Array<IOpenSourceLicense>;
    vendor:                          string;
    logo?:                           string;
    how_to_use?:                     string;
    more_information?:               string;
    source_code_repository?:         string;
}

// OCPI Computer Science Extension!
interface IOpenSourceLicense {
    id:                              string;
    description?:                    Array<IDisplayText>;
    urls?:                           Array<string>;
}

interface IOpenDataLicense {
    id:                              string;
    description?:                    Array<IDisplayText>;
    urls?:                           Array<string>;
}
