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
//  OCPI v2.1.1
//  https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/
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

interface IVersion {
    version:                         VersionNumber;                 // The version number.
    url:                             string;                        // URL to the endpoint containing version specific information.
}

interface IVersionDetail {
    version:                         VersionNumber;                 // The version number.
    endpoints:                       Array<IEndpoint>;              // A list of supported endpoints for this version.
}

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

interface IEndpoint {
    identifier:                     ModuleId;                       // The identifier of the endpoint.
    url:                            string;                         // URL to the endpoint.
}

interface ILocation {
    country_code:                   string;                         // NOT DEFINED IN OCPI v2.1.1!!! The ISO 3166-1 alpha-2 country code of the location.
    party_id:                       string;                         // NOT DEFINED IN OCPI v2.1.1!!! The identifier of the party that owns this location.
    id:                             string;                         // Uniquely identifies the location within the CPOs platform (and suboperator platforms). This field can never be changed, modified or renamed.
    type:                           string;                         // Type of the location.
    name?:                          string;                         // Display name of the location.
    address:                        string;                         // Street/block name and house number if available.
    city:                           string;                         // City name.
    postal_code:                    string;                         // Postal code of the location.
    country:                        string;                         // ISO 3166-1 alpha-3 code for the country of this location.
    coordinates:                    IGeoLocation;                   // Coordinates of the location.
    related_locations?:             Array<IAdditionalGeoLocation>;  // Geographical location of related points relevant to the user.
    evses?:                         Array<IEVSE>;                   // List of EVSEs that belong to this Location.
    directions?:                    Array<IDisplayText>;            // Human-readable directions on how to reach the location.
    operator?:                      IBusinessDetails;               // Information of the operator. When not specified, the information retrieved from the api_info endpoint should be used instead.
    suboperator?:                   IBusinessDetails;               // Information of the suboperator if available.
    owner?:                         IBusinessDetails;               // Information of the owner if available.
    facilities?:                    Array<string>;                  // Optional list of facilities this charge location directly belongs to.
    time_zone?:                     string;                         // One of IANA tzdata's TZ-values representing the time zone of the location. Examples: "Europe/Oslo", "Europe/Zurich". (http://www.iana.org/time-zones)
    opening_times?:                 string;                         // The times when the EVSEs at the location can be accessed for charging.
    charging_when_closed?:          boolean;                        // Indicates if the EVSEs are still charging outside the opening hours of the location. E.g. when the parking garage closes its barriers over night, is it allowed to charge till the next morning? Default: true
    images?:                        Array<IImage>;                  // Links to images related to the location such as photos or logos.
    energy_mix?:                    IEnergyMix;                     // Details on the energy supplied at this location.
    publish?:                       boolean;                        // An OCPI v2.1.1 PlugSurfing extension!
    created?:                       string;                         // Optional timestamp when this location was created [OCPI Computer Science Extension!]
    last_updated:                   string;                         // Timestamp when this Location or one of its EVSEs or Connectors were last updated (or created).
}

interface ILocationMetadata extends TMetadataDefaults {

}

interface IGeoLocation {
    latitude:                       string;                         // Latitude of the point in decimal degree.  Example:   50.770774. Decimal separator: "." Regex: -?[0-9]{1,2}\.[0-9]{6}
    longitude:                      string;                         // Longitude of the point in decimal degree. Example: -126.104965. Decimal separator: "." Regex: -?[0-9]{1,3}\.[0-9]{6}
}

interface IAdditionalGeoLocation {
    latitude:                       string;                         // Latitude of the point in decimal degree.  Example:   50.770774. Decimal separator: "." Regex: -?[0-9]{1,2}\.[0-9]{6}
    longitude:                      string;                         // Longitude of the point in decimal degree. Example: -126.104965. Decimal separator: "." Regex: -?[0-9]{1,3}\.[0-9]{6}
    name?:                          IDisplayText                    // Name of the point in local language or as written at the location. For example the street name of a parking lot entrance or it's number.
}

interface IDisplayText {
    language:                       string;                         // Language Code ISO 639-1
    text:                           string;                         // Text to be displayed to a end user. No markup, html etc. allowed.
}

interface IBusinessDetails {
    name:                           string;                         // Name of the operator.
    website?:                       string;                         // Link to the operator's website.
    logo?:                          string;                         // Image link to the operator's logo.
}

interface IImage {
    url:                            string;                         // URL from where the image data can be fetched through a web browser.
    thumbnail?:                     string;                         // URL from where a thumbnail of the image can be fetched through a webbrowser.
    category:                       string;                         // Describes what the image is used for.
    type:                           string;                         // Image type like: gif, jpeg, png, svg
    width?:                         string;                         // Width of the full scale image
    height?:                        string;                         // Height of the full scale image
}

interface IEnergyMix {
    is_green_energy:                boolean;                        // True if 100% from regenerative sources. (CO2 and nuclear waste is zero)
    energy_sources?:                Array<IEnergySource>;           // Key-value pairs (enum + percentage) of energy sources of this location's tariff.
    environ_impact?:                Array<IEnvironmentalImpact>;    // Key-value pairs (enum + percentage) of nuclear waste and CO2 exhaust of this location's tariff.
    supplier_name?:                 string;                         // Name of the energy supplier, delivering the energy for this location or tariff.*
    energy_product_name?:           string;                         // Name of the energy suppliers product/tariff plan used at this location.*
}

interface IEnergySource {
    source:                         string;                         // The type of energy source.
    percentage:                     number;                         // Percentage of this source (0-100) in the mix.
}

interface IEnvironmentalImpact {
    category:                       string;                         // The category of this value.
    amount:                         number;                         // Amount of this portion in g/kWh.
}

type Capability =
    "CHARGING_PROFILE_CAPABLE"  |                                   // The EVSE supports charging profiles.Sending Charging Profiles is not yet supported by OCPI.
    "CREDIT_CARD_PAYABLE"       |                                   // Charging at this EVSE can be payed with credit card.
    "REMOTE_START_STOP_CAPABLE" |                                   // The EVSE can remotely be started/stopped.
    "RESERVABLE"                |                                   // The EVSE can be reserved.
    "RFID_READER"               |                                   // Charging at this EVSE can be authorized with a RFID token
    "UNLOCK_CAPABLE"            |                                   // Connectors have mechanical lock that can be requested by the eMSP to be unlocked.
     string;

type ConnectorFormat =
    "SOCKET" |                                                      // The connector is a socket; the EV user needs to bring a fitting plug.
    "CABLE";                                                        // The connector is an attached cable; the EV users car needs to have a fitting inlet.

type ConnectorType =
    "CHADEMO"               |                                       // The connector type is CHAdeMO, DC
    "DOMESTIC_A"            |                                       // Standard/Domestic household, type "A", NEMA 1-15, 2 pins
    "DOMESTIC_B"            |                                       // Standard / Domestic household, type "B", NEMA 5 - 15, 3 pins
    "DOMESTIC_C"            |                                       // Standard / Domestic household, type "C", CEE 7 / 17, 2 pins
    "DOMESTIC_D"            |                                       // Standard / Domestic household, type "D", 3 pin
    "DOMESTIC_E"            |                                       // Standard / Domestic household, type "E", CEE 7 / 5 3 pins
    "DOMESTIC_F"            |                                       // Standard / Domestic household, type "F", CEE 7 / 4, Schuko, 3 pins
    "DOMESTIC_G"            |                                       // Standard / Domestic household, type "G", BS 1363, Commonwealth, 3 pins
    "DOMESTIC_H"            |                                       // Standard / Domestic household, type "H", SI - 32, 3 pins
    "DOMESTIC_I"            |                                       // Standard / Domestic household, type "I", AS 3112, 3 pins
    "DOMESTIC_J"            |                                       // Standard / Domestic household, type "J", SEV 1011, 3 pins
    "DOMESTIC_K"            |                                       // Standard / Domestic household, type "K", DS 60884 - 2 - D1, 3 pins
    "DOMESTIC_L"            |                                       // Standard / Domestic household, type "L", CEI 23 - 16 - VII, 3 pins
    "IEC_60309_2_single_16" |                                       // IEC 60309 - 2 Industrial Connector single phase 16 Amperes(usually blue)
    "IEC_60309_2_three_16"  |                                       // IEC 60309 - 2 Industrial Connector three phase 16 Amperes(usually red)
    "IEC_60309_2_three_32"  |                                       // IEC 60309 - 2 Industrial Connector three phase 32 Amperes(usually red)
    "IEC_60309_2_three_64"  |                                       // IEC 60309 - 2 Industrial Connector three phase 64 Amperes(usually red)
    "IEC_62196_T1"          |                                       // IEC 62196 Type 1 "SAE J1772"
    "IEC_62196_T1_COMBO"    |                                       // Combo Type 1 based, DC
    "IEC_62196_T2"          |                                       // IEC 62196 Type 2 "Mennekes"
    "IEC_62196_T2_COMBO"    |                                       // Combo Type 2 based, DC
    "IEC_62196_T3A"         |                                       // IEC 62196 Type 3A
    "IEC_62196_T3C"         |                                       // IEC 62196 Type 3C "Scame"
    "TESLA_R"               |                                       // Tesla Connector "Roadster" - type(round, 4 pin)
    "TESLA_S"               |                                       // Tesla Connector "Model-S" - type(oval, 5 pin)
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
    "BUS_STOP"        |                                             // A bus stop.
    "TAXI_STAND"      |                                             // A taxi stand.
    "TRAIN_STATION"   |                                             // A train station.
    "AIRPORT"         |                                             // An airport.
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

type LocationType =
    "ON_STREET"          |                                          // Parking in public space.
    "PARKING_GARAGE"     |                                          // Multistorey car park.
    "UNDERGROUND_GARAGE" |                                          // Multistorey car park, mainly underground.
    "PARKING_LOT"        |                                          // A cleared area that is intended for parking vehicles, i.e.at super markets, bars, etc.
    "OTHER"              |                                          // None of the given possibilities.
    "UNKNOWN"            |                                          // Parking location type is not known by the operator (default).
     string;

type ParkingRestriction =
    "EV_ONLY"     |                                                 // Reserved parking spot for electric vehicles.
    "PLUGGED"     |                                                 // Parking is only allowed while plugged in (charging).
    "DISABLED"    |                                                 // Reserved parking spot for disabled people with valid ID.
    "CUSTOMERS"   |                                                 // Parking spot for customers/guests only, for example in case of a hotel or shop.
    "MOTORCYCLES" |                                                 // Parking spot only suitable for (electric) motorcycles or scooters.
     string;

type PowerType =
    "AC_1_PHASE" |                                                  // AC mono phase.
    "AC_3_PHASE" |                                                  // AC 3 phase.
    "DC";                                                           // Direct Current.

type Status  =
    "AVAILABLE"   |                                                 // The EVSE/Connector is able to start a new charging session.
    "BLOCKED"     |                                                 // The EVSE / Connector is not accessible because of a physical barrier, i.e.a car.
    "CHARGING"    |                                                 // The EVSE / Connector is in use.
    "INOPERATIVE" |                                                 // The EVSE / Connector is not yet active or it is no longer available(deleted).
    "OUTOFORDER"  |                                                 // The EVSE / Connector is currently out of order.
    "PLANNED"     |                                                 // The EVSE / Connector is planned, will be operating soon
    "REMOVED"     |                                                 // The EVSE / Connector / charge point is discontinued / removed.
    "RESERVED"    |                                                 // The EVSE / Connector is reserved for a particular EV driver and is unavailable for other drivers.
    "UNKNOWN"     |                                                 // No status information available. (Also used when offline)
     string;

interface IEVSE {
    uid:                            string;                         // Uniquely identifies the EVSE within the CPOs platform (and suboperator platforms). For example a database unique ID or the "EVSE ID". This field can never be changed, modified or renamed. This is the 'technical' identification of the EVSE, not to be used as 'human readable' identification, use the field: evse_id for that.
    evse_id?:                       string;                         // Compliant with the following specification for EVSE ID from "eMI3 standard version V1.0" (http://emi3group.com/documents-links/) "Part 2: business objects." Optional because: if an EVSE ID is to be re-used the EVSE ID can be removed from an EVSE that is removed (status: REMOVED)
    status:                         string;                         // Indicates the current status of the EVSE.
    status_schedule?:               Array<IStatusSchedule>;         // Indicates a planned status in the future of the EVSE.
    capabilities?:                  Array<string>;                  // List of functionalities that the EVSE is capable of.
    connectors:                     Array<IConnector>;              // List of available connectors on the EVSE.
    energy_meter?:                  IEnergyMeter;                   // Optional energy meter [OCPI Computer Science Extension!]
    floor_level?:                   string;                         // Level on which the charging station is located (in garage buildings) in the locally displayed numbering scheme.
    coordinates:                    IGeoLocation;                   // Coordinates of the EVSE.
    physical_reference?:            string;                         // A number/string printed on the outside of the EVSE for visual identification.
    directions?:                    Array<IDisplayText>;            // Multi-language human-readable directions when more detailed information on how to reach the EVSE from the Location is required.
    parking_restrictions?:          Array<string>;                  // The restrictions that apply to the parking spot.
    images?:                        Array<IImage>;                  // Links to images related to the EVSE such as photos or logos.
    created?:                       string;                         // Optional timestamp when this EVSE was created [OCPI Computer Science Extension!]
    last_updated:                   string;                         // Timestamp when this EVSE or one of its Connectors was last updated (or created).
}

interface IStatusSchedule {
    period_begin:                   string;                         // Begin of the scheduled period.
    period_end?:                    string;                         // End of the scheduled period, if known.
    status:                         string;                         // Status value during the scheduled period.
}

interface IConnector {
    id:                             string;                         // Identifier of the connector within the EVSE. Two connectors may have the same id as long as they do not belong to the same EVSE object.
    standard:                       string;                         // The standard of the installed connector.
    format:                         string;                         // The format (socket/cable) of the installed connector.
    power_type:                     string;                         // The power type of the connector.
    voltage:                        number;                         // Voltage of the connector (line to neutral for AC_3_PHASE), in volt [V].
    amperage:                       number;                         // Maximum amperage of the connector, in ampere [A].
    tariff_id?:                     string;                         // Identifier of the current charging tariff structure. For a "Free of Charge" tariff this field should be set, and point to a defined "Free of Charge" tariff.
    terms_and_conditions?:          string;                         // URL to the operator's terms and conditions.
    created?:                       string;                         // Optional timestamp when this connector was created [OCPI Computer Science Extension!]
    last_updated:                   string;                         // Timestamp when this Connectors was last updated (or created).
}


interface ITariff {
    id:                             string,                         // Uniquely identifies the tariff within the CPOs platform(and suboperator platforms).
    currency:                       string,                         // Currency of this tariff, ISO 4217 Code
    tariff_alt_text:                Array<IDisplayText>,            // List of multi language alternative tariff info text
    tariff_alt_url?:                string,                         // Alternative URL to tariff info
    elements:                       Array<ITariffElement>,          // List of tariff elements
    energy_mix?:                    IEnergyMix,                     // Details on the energy supplied with this tariff.
    created?:                       string;                         // Optional timestamp when this Tariff was created [OCPI Computer Science Extension!]
    last_updated:                   string                          // Timestamp when this tariff was last updated(or created).
}
interface ITariffMetadata extends TMetadataDefaults {

}

interface ITariffElement {
    price_components:               Array<IPriceComponent>,         // List of price components that make up the pricing of this tariff
    restrictions?:                  ITariffRestrictions             // Tariff restrictions object
}

type TariffDimension =
    "ENERGY"       |                                                // defined in kWh, step_size multiplier: 1 Wh
    "FLAT"         |                                                // flat fee, no unit
    "PARKING_TIME" |                                                // time not charging: defined in hours, step_size multiplier: 1 second
    "TIME";                                                         // time charging: defined in hours, step_size multiplier: 1 second

interface IPriceComponent {
    type:                           TariffDimension,                // Type of tariff dimension
    price:                          number,                         // price per unit(excluding VAT) for this tariff dimension
    step_size:                      number                          // Minimum amount to be billed.This unit will be billed in this step_size blocks.For example: if type is time and step_size is 300, then time will be billed in blocks of 5 minutes, so if 6 minutes is used, 10 minutes(2 blocks of step_size) will be billed.
}

type DayOfWeek =
    "MONDAY"    |
    "TUESDAY"   |
    "WEDNESDAY" |
    "THURSDAY"  |
    "FRIDAY"    |
    "SATURDAY"  |
    "SUNDAY";

interface ITariffRestrictions {
    start_time?:                    string,                         // Start time of day, for example 13: 30, valid from this time of the day.Must be in 24h format with leading zeros.Hour / Minute separator: ":" Regex: ([0 - 1][0 - 9] | 2[0 - 3]): [0 - 5][0 - 9]
    end_time?:                      string,                         // End time of day, for example 19: 45, valid until this time of the day.Same syntax as start_time
    start_date?:                    string,                         // Start date, for example: 2015 - 12 - 24, valid from this day
    end_date?:                      string,                         // End date, for example: 2015 - 12 - 27, valid until this day(excluding this day)
    min_kwh?:                       number,                         // Minimum used energy in kWh, for example 20, valid from this amount of energy is used
    max_kwh?:                       number,                         // Maximum used energy in kWh, for example 50, valid until this amount of energy is used
    min_power?:                     number,                         // Minimum power in kW, for example 0, valid from this charging speed
    max_power?:                     number,                         // Maximum power in kW, for example 20, valid up to this charging speed
    min_duration?:                  number,                         // Minimum duration in seconds, valid for a duration from x seconds
    max_duration?:                  number,                         // Maximum duration in seconds, valid for a duration up to x seconds
    day_of_week?:                   Array<DayOfWeek>                // * Which day(s) of the week this tariff is valid
}


type TokenType =
    "OTHER" |                                                       // Other type of token
    "RFID"  |                                                       // RFID Token
     string;

type Allowed =
    "ALLOWED"     |                                                 // This Token is allowed to charge at this location.
    "BLOCKED"     |                                                 // This Token is blocked.
    "EXPIRED"     |                                                 // This Token has expired.
    "NO_CREDIT"   |                                                 // This Token belongs to an account that has not enough credits to charge at the given location.
    "NOT_ALLOWED" |                                                 // Token is valid, but is not allowed to charge at the given location.
     string;

type WhitelistType =
    "ALWAYS"          |                                             // Token always has to be whitelisted, realtime authorization is not possible/allowed.
    "ALLOWED"         |                                             // It is allowed to whitelist the token, realtime authorization is also allowed.
    "ALLOWED_OFFLINE" |                                             // Whitelisting is only allowed when CPO cannot reach the eMSP (communication between CPO and eMSP is offline)
    "NEVER";                                                        // Whitelisting is forbidden, only realtime authorization is allowed. Token should always be authorized by the eMSP.

interface IToken {
    uid:                            string;                         // Identification used by CPO system to identify this token. Currently, in most cases, this is the RFID hidden ID as read by the RFID reader.
    type:                           TokenType;                      // Type of the token
    auth_id:                        string;                         // Uniquely identifies the EV Driver contract token within the eMSP's platform (and suboperator platforms). Recommended to follow the specification for eMA ID from "eMI3 standard version V1.0" (http://emi3group.com/documents-links/) "Part 2: business objects."
    visual_number?:                 string;                         // Visual readable number/identification as printed on the Token (RFID card), might be equal to the auth_id.
    issuer:                         string;                         // Issuing company, most of the times the name of the company printed on the token (RFID card), not necessarily the eMSP.
    valid:                          boolean;                        // Is this Token valid
    whitelist:                      WhitelistType;                  // Indicates what type of white-listing is allowed.
    language?:                      string;                         // Language Code ISO 639-1. This optional field indicates the Token owner's preferred interface language. If the language is not provided or not supported then the CPO is free to choose its own language.
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
    "ACTIVE"    |                                                   // The session is accepted and active. Al pre-condition are met: Communication between EV and EVSE (for example: cable plugged in correctly), EV or Driver is authorized. EV is being charged, or can be charged. Energy is, or is not, being transfered.
    "COMPLETED" |                                                   // The session is finished successfully. No more modifications will be made to this session.
    "INVALID"   |                                                   // The session is declared invalid and will not be billed.
    "PENDING"   |                                                   // The session is pending, it has not yet started. Not all pre-condition are met. This is the initial state. This session might never become an active session.
     string;

interface ICDRDimension {
    type:                           CdrDimensionType;               // Type of cdr dimension
    volume:                         number;                         // Volume of the dimension consumed, measured according to the dimension type.
}

interface IChargingPeriod {
    start_date_time:                string;                         // Start date and time of this charging period.
    dimensions:                     Array<ICDRDimension>;           // List of dimensions for this charging period.
}

interface ISession {
    id:                             string;                         // The unique id that identifies the session in the CPO platform.
    start_datetime:                 string;                         // The time when the session became active.
    end_datetime?:                  string;                         // The time when the session is completed.
    kwh:                            number;                         // How many kWh are charged.
    auth_id:                        string;                         // Reference to a token, identified by the auth_id field of the Token.
    auth_method:                    AuthMethod;                     // Method used for authentication.
    location:                       ILocation;                      // The location where this session took place, including only the relevant EVSE and connector.
    meter_id?:                      string;                         // Optional identification of the kWh meter.
    currency:                       string;                         // ISO 4217 code of the currency used for this session.
    charging_periods?:              Array<IChargingPeriod>;         // Optional list of charging periods.
    total_cost?:                    number;                         // The total cost (excluding VAT) of the session in the specified currency. This is the price that the eMSP will have to pay to the CPO. A total_cost of 0.00 means free of charge. When omitted, no price information is given in the Session object, this does not have to mean it is free of charge.
    status:                         SessionStatus;                  // The status of the session.
    created?:                       string;                         // Optional timestamp when this session was created [OCPI Computer Science Extension!]
    last_updated:                   string;                         // Timestamp when this session was last updated (or created).
}

interface ISessionMetadata extends TMetadataDefaults {

}


interface ICDR {
    id:                             string;                         // Uniquely identifies the CDR within the CPOs platform (and suboperator platforms).
    start_date_time:                string;                         // Start timestamp of the charging session.
    stop_date_time:                 string;                         // Stop timestamp of the charging session.
    auth_id:                        string;                         // Reference to a token, identified by the auth_id field of the Token.
    auth_method:                    AuthMethod;                     // Method used for authentication.
    location:                       ILocation;                      // Location where the charging session took place, including only the relevant EVSE and Connector.
    meter_id?:                      string;                         // Identification of the Meter inside the Charge Point.
    currency:                       string;                         // Currency of the CDR in ISO 4217 Code.
    tariffs?:                       Array<ITariff>;                 // List of relevant tariff elements, see: Tariffs. When relevant, a "Free of Charge" tariff should also be in this list, and point to a defined "Free of Charge" tariff.
    charging_periods:               Array<IChargingPeriod>;         // List of charging periods that make up this charging session. A session consists of 1 or more periods, where each period has a different relevant Tariff.
    total_cost:                     number;                         // Total cost (excluding VAT) of this transaction.
    total_energy:                   number;                         // Total energy charged, in kWh.
    total_time:                     number;                         // Total duration of this session (including the duration of charging and not charging), in hours.
    total_parking_time?:            number;                         // Total duration during this session that the EV is not being charged (no energy being transfered between EVSE and EV), in hours.
    remark?:                        string;                         // Optional remark, can be used to provide addition human readable information to the CDR, for example: reason why a transaction was stopped.
    created?:                       string;                         // Optional timestamp when this CDR was created [OCPI Computer Science Extension!]
    last_updated:                   string;                         // Timestamp when this CDR was last updated (or created).
}

interface ICDRMetadata extends TMetadataDefaults {

}



// -----------------------------
// OCPI Management Extensions!
// -----------------------------

interface IRemoteParty {
    //id:                              string;
    //"@context":                      string;
    countryCode:                     string;
    partyId:                         string;
    role:                            string;
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
