# POI3 ↔ OCPI 2.3.0

A POI3 roaming network ([WWCP_POI](https://github.com/OpenChargingCloud/WWCP_POI), `WWCP_POI3`) as OCPI 2.3.0
locations, EVSEs, connectors and tariffs, and back.

| Class | What it does |
|---|---|
| `POI3Mapper` | The mapping itself: pure conversions in both directions, with warnings for what does not map. |
| `POI3ToOCPIPublisher` | **CPO side:** a `RoamingNetworkHistory` feeds an OCPI `CommonAPI`, which serves its locations and tariffs to EMSPs. |
| `OCPIToPOI3Importer` | **EMSP or hub side:** what an OCPI `CommonAPI` receives, e.g. pushed by CPOs, is committed to a `RoamingNetworkHistory`. |

## Usage

```csharp
// CPO side: publish the head, and every commit and EVSE status change from now on.
await using var publisher = new POI3ToOCPIPublisher(history, commonAPI, OperatorIds: [ ChargingStationOperator_Id.Parse("DE*GEF") ]);
publisher.OnError += (sender, description, warnings) => …;
await publisher.StartAsync();

// EMSP/hub side: commit what the CommonAPI receives, signed with the keys of the operators.
await using var importer = new OCPIToPOI3Importer(commonAPI, history, changeSet => changeSet.Sign(operatorKey, operatorKeyId, COSEAlgorithm.ES256));
importer.OnError += (sender, description, warnings) => …;
importer.Start();
await importer.ImportAsync(locationHeldBefore);   // e.g. what the CommonAPI held before the start
```

- **Publisher:** at the start, every charging pool and charging tariff of the selected operators (all, unless given) is
  stored with `AddOrUpdateLocation`/`AddOrUpdateTariff`. Every commit maps the head again: what changed is stored, what
  is gone is removed (`RemoveLocation`/`RemoveTariff`). Every change of the status or admin status of an EVSE stores its
  OCPI EVSE (`AddOrUpdateEVSE`). `last_updated` is always later than what the CommonAPI holds, which it requires.
  The parties of the operators must be parties of the CommonAPI, and what it holds for them is the publisher's: at the
  start, what is no longer in the head is removed, e.g. what was removed while no publisher ran. Without `OperatorIds`
  these are the operators of the head, so the data of an operator removed in the meantime stays.
  Pushing to remote EMSPs is the CommonAPI's business:
  the publisher only keeps its store up to date, which EMSPs pull from. It publishes the effective charging pools,
  stations and EVSEs, with the properties their groups give them (`EffectiveChargingPools`), e.g. the facilities of a
  pool, a feature `Reservable` or a physical reference of a station, the connectors of an EVSE, given once for all
  members of a group; a change of a group changes the locations of its members, its `lastChange` being the last change
  of these and of what embeds them. New prices of the price status of charging pools, stations and EVSEs are tariffs of
  their own, which the EVSEs name (see [Prices at runtime](#prices-at-runtime--tariffs-of-their-own)); a change of a
  price status publishes the head again, once for all reported until then.
- **Importer:** every location or tariff added or changed becomes a change set against the head: the charging pool or
  charging tariff as OCPI tells it, onto the one there is, with what only POI3 knows kept (see below). The difference is
  computed as a three-way merge whose left side is its base (`RoamingNetwork.TryMerge(head, head, desired)`), so only what
  changed is in the change set; nothing changed, nothing is committed. It is signed by the given delegate, since an
  operator owns its data and every change set must be signed by its owners, and committed. A removed location or tariff
  removes its charging pool or charging tariff, and an EVSE whose status is `REMOVED` its EVSE (the CommonAPI keeps such
  EVSEs by default). EVSE status goes into the runtime status of the history (`DataSource` "OCPI"), and so do prices:
  a tariff changed in its elements only is not committed, its prices become the price status of the charging pools
  and EVSEs naming it (see [Prices at runtime](#prices-at-runtime--tariffs-of-their-own)).
  The operators must be in the roaming network already: adding them is up to the roaming network operator.
- Both work one job after the other, in the order the events came. **Do not use both on the same history and
  CommonAPI:** each would feed the other's changes back.

## Mapping

### Identifications

| POI3 | OCPI 2.3.0 |
|---|---|
| operator `DE*GEF` | `country_code` `DE`, `party_id` `GEF` |
| charging pool `DE*GEF*P1` | location `id` `1` |
| EVSE `DE*GEF*E1*1` | EVSE `uid` and `evse_id` `DE*GEF*E1*1` (import: from `evse_id`, else `uid`) |
| charging connector `1` | connector `id` `1` |
| charging tariff `DE*GEF*T1` | tariff `id` `1` |
| charging tariff `DE*GEF*T1` with new prices of a price status | tariff `id` `1~` + the first 8 hex digits of the SHA-256 of the compact JSON of their elements, e.g. `1~3fa9c10b` (no charging tariff identification has a `~`) |
| charging station | — (import: new EVSEs go into the first charging station of the pool, or into `DE*GEF*S1` named after the location) |

### Charging pool ↔ location

| POI3 charging pool | OCPI location |
|---|---|
| — | `publish` = true |
| `address`: street + house number, postal code, city, country, region | `address`, `postal_code`, `city`, `country`, `state` (import: the whole `address` becomes the street) |
| `geoLocation` | `coordinates` |
| `timeZone`, else the time zone of the address, else UTC | `time_zone` |
| `name` (first text) | `name` (import: in the given language) |
| `parkingType` | `parking_type` |
| `relatedLocations` | `related_locations` (first text of the name) |
| `arrivalInstructions` | `directions` |
| operator: name, homepage, logo | `operator` (`BusinessDetails`) |
| first brand: name, homepage, logo | `suboperator` |
| `facilities` | `facilities` |
| `openingTimes` (none: 24/7) | `opening_times` |
| `chargingWhenClosed` | `charging_when_closed` |
| `images` | `images` (content type ↔ file type, category by name) |
| `energyMix`: energy sources, supplier, product | `energy_mix` (`is_green_energy` when every source is green): Gas → `GAS`, Coal/StoneCoal → `COAL`, Oil/OtherFossil → `GENERAL_FOSSIL`, NuclearFision/NuclearFusion → `NUCLEAR`, Sun → `SOLAR`, Wind → `WIND`, Geothermal/Biogas/OtherGreen → `GENERAL_GREEN` (import: `GAS` → Gas, `COAL` → Coal, `GENERAL_FOSSIL` → OtherFossil, `NUCLEAR` → NuclearFision, `SOLAR` → Sun, `WIND` → Wind, `WATER`/`GENERAL_GREEN` → OtherGreen); environmental impacts in g/kWh: CarbonDioxide ↔ `CARBON_DIOXIDE`, NuclearWaste ↔ `NUCLEAR_WASTE`, others are left out with a warning |
| `hotline`, else the operator's | `help_phone` |
| `services`, `standards` (the same names) | `services`, `standards` (Accessibility Extension) |
| `assistanceServiceDetails` (first text) | `assistance_service_details` (import: in the given language) |
| `created`, latest `lastChange` of pool, stations, EVSEs and EVSE status | `created`, `last_updated` |

### Charging station + EVSE ↔ EVSE

| POI3 | OCPI EVSE |
|---|---|
| EVSE status, and admin status where not operational: planned/inDeployment → `PLANNED`, deleted → `REMOVED`, outOfService → `INOPERATIVE`, blocked → `BLOCKED` | `status` |
| pool authentication modes, payment options, UI features; station features | `capabilities`: RFID → `RFID_READER`, CreditCard → `CREDIT_CARD_PAYABLE`, DebitCard → `DEBIT_CARD_PAYABLE`, NFC → `CONTACTLESS_CARD_SUPPORT`, PINPAD → `PED_TERMINAL`, REMOTE → `REMOTE_START_STOP_CAPABLE`, Reservable → `RESERVABLE`, ChargingProfilesSupported → `CHARGING_PROFILE_CAPABLE`, ChargingPreferencesSupported → `CHARGING_PREFERENCES_CAPABLE`, TokenGroupsSupported → `TOKEN_GROUP_CAPABLE`, CSOUnlockSupported → `UNLOCK_CAPABLE` (import: for new pools only) |
| EVSE `geoLocation`, else the station's, else the pool's | `coordinates` |
| EVSE `physicalReference`, else the station's | `physical_reference` |
| pool `parkingRestrictions` (TAXI ↔ TAXIS) | `parking_restrictions` |
| EVSE and station `images` | `images` |
| `reachDistance` (m), `operationTimeout` (s), `extendedOperationTimeout`, `standards` | `reach_distance` (cm), `operation_timeout` (s), `extended_operation_timeout`, `standards` (Accessibility Extension) |
| `floorLevel` | `floor_level` |
| `acceptedEMobilityProviderIds`: `DE-GDF`, also as `DE*GDF` | `accepted_service_providers`: `DE-GDF` (import: an eMSP that is no e-mobility provider is left out with a warning) |

| EVSE status | OCPI status |
|---|---|
| available, waitingForPlugin | `AVAILABLE` |
| reserved | `RESERVED` |
| charging, pluggedIn, occupied | `CHARGING` |
| blocked | `BLOCKED` |
| offline, unavailable, inoperative, outOfService, doorNotClosed | `INOPERATIVE` |
| error, faulted, outOfOrder | `OUTOFORDER` |
| inDeployment | `PLANNED` |
| removed | `REMOVED` |
| anything else | `UNKNOWN` |

Import: `AVAILABLE` → available, `BLOCKED` → blocked, `CHARGING` → charging, `INOPERATIVE` → inoperative,
`OUTOFORDER` → outOfOrder, `PLANNED` → inDeployment, `REMOVED` → removed, `RESERVED` → reserved, else unknown.

### Parking space ↔ parking

| POI3 | OCPI 2.3 |
|---|---|
| the parking spaces the charging stations of a pool name (`parkingSpaceIds`), of parking operators or of the charging station operator | location `parking_places` |
| the `parkingSpaceIds` of the EVSE's charging station | EVSE `parking` (`parking_id`; import: the union of its EVSEs' for a charging station) |
| `@id` | `id` |
| `physicalReference`, `vehicleTypes`, `restrictedToVehicleTypes` (unknown: false), `reservationRequired` (unknown: false) | `physical_reference`, `vehicle_types`, `restricted_to_type`, `reservation_required` |
| `maxVehicleWeight` (kg), `maxVehicleHeight`/`Length`/`Width`, `length`, `width` (m) | `max_vehicle_weight`, `max_vehicle_height`/`length`/`width`, `parking_space_length`/`width` (import: measures not positive are left out) |
| `evsePosition`, `direction`, `surface`, `slope` | `evse_position`, `direction`, `surface`, `slope` |
| `timeLimit` (s) | `time_limit` |
| `driveThrough`, `roofed`, `illuminated`, `refrigerationOutlet`, `dangerousGoodsAllowed`, `protectedArea` | `drive_through`, `roofed`, `lighting`, `refrigeration_outlet`, `dangerous_goods_allowed`, `protected_area` |
| `apdsReference`, `standards`, `images` | `apds_reference`, `standards`, `images` |

Import: a parking place a parking operator defines is named by the charging station, not changed: the parking operator
owns it. Every other one is a parking space the charging station operator defines itself, added or, with what only POI3
knows kept and what OCPI tells alike as well, changed. One another charging station operator defines is left out with a
warning, and so is a `parking_id` of an EVSE that is no parking space. Parking spaces no location names any more stay.

### Charging connector ↔ connector

| POI3 | OCPI connector |
|---|---|
| connector `type` | `standard` (the names are OCPI's) |
| `format` (absent: `SOCKET`) | `format` |
| `cableLength` (m), `cableWeight` (kg), `cableManagementSystem`, `standards`, `images` | `cable_length` (cm), `cable_weight` (kg), `cable_management_system`, `standards`, `images` (Accessibility Extension; import: the cable of a `SOCKET` is left out with a warning) |
| EVSE `currentTypes`: DC, AC_ThreePhases, AC_OnePhase | `power_type`: `DC`, `AC_3_PHASE`, `AC_1_PHASE` |
| EVSE `maxVoltage`, else 230 V AC / 400 V DC | `max_voltage` |
| EVSE `maxCurrent`, else 16 A AC / 50 A DC | `max_amperage` |
| EVSE `maxPower` | `max_electric_power` |
| EVSE `chargingTariffIds`, else the `chargingTariffId` of its most specific EVSE, charging station or charging pool groups, as its effective EVSE has them | `tariff_ids` |
| operator `termsAndConditionsURL` | `terms_and_conditions` |

Import: the EVSE takes the union of the current types of its connectors, the maximum of their voltages, amperages and
powers, and the union of their tariffs.

### Charging tariff ↔ tariff

| POI3 | OCPI tariff |
|---|---|
| `currency` | `currency` |
| prices excluding VAT | `tax_included` = `NO` (import: prices of `YES` are divided by their VAT, if any, and rounded to six decimal places; `N/A` means no VAT, written as 0 % where none is given) |
| `name`, `description` | `tariff_alt_text` (import: into `name`) |
| `tariffURL` | `tariff_alt_url` |
| `tariffType` | `type` (the same names; import: a type POI3 does not know is left out with a warning) |
| `validFrom`, `validUntil` | `start_date_time`, `end_date_time` (import: an end not after the start refuses the tariff with a warning) |
| `minPrice`, `maxPrice`: excluding VAT, including VAT | `min_price`, `max_price`: `before_taxes`, `after_taxes` (import: a minimum above the maximum refuses the tariff with a warning) |
| `energyMix` | `energy_mix` (energy sources and environmental impacts as for locations) |
| element: price components, restrictions | `elements` |
| price component `ENERGY`, `TIME`, `PARKING_TIME`, `FLAT`: price, VAT, step size | `ENERGY` (per kWh, step in Wh), `TIME`/`PARKING_TIME` (per hour, step in s), `FLAT` (step 1) |
| restrictions: start/end time, start/end date, min/max energy (Wh), min/max current, min/max power (W), min/max duration (s), days of week, reservation | `start_time`, `end_time`, `start_date`, `end_date`, `min_kwh`/`max_kwh` (kWh), `min_current`/`max_current`, `min_power`/`max_power` (kW), `min_duration`/`max_duration`, `day_of_week`, `reservation` (`RESERVATION`, `RESERVATION_EXPIRES`) |

### Prices at runtime ↔ tariffs of their own

The price status of a charging pool, charging station or EVSE gives charging tariffs new elements at runtime, e.g. ad
hoc prices following the electricity market, without a commit; at an EVSE the most specific one naming a tariff applies,
its own before its station's before its pool's (`ChargingPrices.Effective`). OCPI 2.3.0 tariffs apply wherever a
connector names them, so:

- **Publisher:** a charging tariff with the new prices in effect at an EVSE is a tariff of its own: the tariff as it is,
  with the elements of the prices and the identification `1~…` of their hash (see above), the same for the same prices
  wherever they apply. Its `last_updated` is the earliest time since when they do. The connectors of the EVSE name it
  instead of the tariff, and the EVSE, its connectors and so the location are updated since the prices apply. A tariff
  of prices in effect nowhere any more is removed; the tariff itself always stays.
- **Importer:** a tariff whose elements only changed is not committed: its elements become the price status of the
  locations naming it (also as `1~…`). Prices the same at every EVSE of a location naming a tariff are the price
  status of its charging pool, others the EVSE's own; where the prices are the tariff's, they are removed from it. The
  price status of a charging station is followed, never set: an EVSE whose station has other prices gets its own. A
  tariff `1~…` names its charging tariff `DE*GEF*T1`, is never a charging tariff of its own, and tells the prices of the
  EVSEs naming it. A tariff changed in more than its elements is committed as a whole, its elements as well. A tariff
  the CommonAPI does not hold (yet) tells nothing: the prices stay as they are. So what a POI3 network publishes,
  imported back into it, sets no price status again.

### What the import keeps

A location imported onto an existing charging pool sets only `name`, `address`, `geoLocation`, `timeZone`, `parkingType`,
`hotline`, `openingTimes`, `energyMix`, `images`, `facilities`, `arrivalInstructions`, `chargingWhenClosed`,
`relatedLocations`, `services`, `assistanceServiceDetails` and `standards`; an existing EVSE only `physicalReference`,
`currentTypes`, `maxVoltage`, `maxCurrent`, `maxPower`, `chargingConnectors`, `chargingTariffIds`, `images`,
`geoLocation`, `reachDistance`, `operationTimeout`, `extendedOperationTimeout`, `standards`, `floorLevel` and
`acceptedEMobilityProviderIds`. Everything else of POI3
stays as it is, e.g. descriptions, data licenses, brands, grid connection, energy meters, authentication modes, features,
the access type (`accessibility`), EVSE charging modes and reservation duration. EVSEs no longer at OCPI, or `REMOVED`, are removed; new ones are added
(see above).

Of these, what OCPI tells as it would tell the existing charging pool, EVSE or charging tariff is kept as it is: the
existing one is mapped to OCPI and back, and only where that differs from what OCPI tells does OCPI's value win. So a
street keeps its house number apart, a city its language, an EVSE no coordinates of its own when it has those of its
station or pool, a tariff its name and description apart, EVSEs the tariffs of their groups, and charging pools,
stations and EVSEs the properties their groups give them: the existing charging pool is mapped as it is at runtime,
its effective one. What a POI3 network publishes, imported back into it, commits nothing.

What OCPI tells of a charging pool or EVSE as its groups give it stays in the groups (`POIGroupMembers.KeepWhatGroupsGive`,
by the rules by which the groups give it at runtime): of an array it unites with theirs, e.g. the facilities of a pool,
it takes as its own only what its groups do not give it, elements matched by their key (an image by its URL), and an array it takes whole - the charging
tariffs and current types of an EVSE - becomes its own whole when OCPI tells another one. So a facility added at OCPI
does not make the facilities of the pool's group its own, and they still go when its group no longer gives them. So do
the parking spaces a charging station's groups give it: its EVSEs' `parking` names them, the import does not make them
its own.

## Gaps

What one side has and the other does not, or not alike:

- **Charging stations:** OCPI 2.3.0 has none. On export a station's properties end up in its EVSEs (physical reference,
  coordinates, images, features as capabilities) or are lost (manufacturer, model, calibration and certification info,
  vehicle types, accessibility, max power, parking spaces, hotline, opening times, brands); on import EVSEs go into existing
  or one new station per location.
- **Connectors:** POI3 has no per-connector voltage, amperage, power or tariffs, and no connector capabilities
  (ISO 15118 Plug & Charge): they come from the EVSE. `lockable` has no OCPI counterpart. A plug type OCPI does not know leaves the connector out (with a warning).
- **Location:** POI3 has no `publish`/`publish_allowed_to` (always published), no `owner`, no energy meters of locations.
  `assistance_service_details` is one text, POI3 has one per language. The house number is joined with the street, and only split
  again where the address is as before (see above).
- **Energy sources:** OCPI has fewer categories than POI3: stone coal, oil, nuclear fusion, geothermal and biogas come back
  as Coal, OtherFossil, NuclearFision and OtherGreen. POI3 has no water, which becomes OtherGreen; OCPI has no waste, which
  is left out (with a warning), as are unknown sources.
- **POI3 only:** descriptions, entrance/exit addresses and locations, location languages, payment options and authentication
  modes beyond the capabilities above, pool features, grid connection points, data licenses, groups, brands (beyond the
  first as sub-operator), energy status of EVSEs and stations, charging pool and station status.
- **Parking:** POI3 names parking spaces by charging stations, OCPI by EVSEs; the `access_level` of an EVSE's `parking`
  has no POI3 counterpart, and its `evse_position` is the parking space's. Names, descriptions, garages, sensors,
  products, `parkingRestrictions` and `euVehicleCategories` of parking spaces have no OCPI counterpart.
- **EVSE:** POI3 has no `status_schedule`; OCPI has no charging modes, reservation duration,
  names or descriptions of EVSEs.
- **Tariffs:** the POI3 price dimensions `MIN_CURRENT`/`MAX_CURRENT` have no OCPI 2.3.0 counterpart (such a tariff is not
  published). An OCPI price component without `vat` has no VAT; a POI3 one without `vat` has an unknown VAT.
  OCPI cannot tell new prices from a changed tariff: a change of the elements of a tariff in POI3, committed, is a price
  status after an import elsewhere. Tariff identifications with new prices may exceed the 36 characters of OCPI where the
  suffix of the charging tariff has more than 27.
- **Payment terminals** (the OCPI 2.3 Payments module) are not mapped.
- **Identifications:** an OCPI party must have three characters to be an operator; a location id must fit a charging pool
  suffix (`[A-Za-z0-9][A-Za-z0-9*]{0,29}`) and a tariff id a charging tariff suffix; others are refused with a warning.
- **Import of groups and operators:** the tariff an EVSE without tariffs of its own takes from its EVSE, station or
  pool groups becomes the connector tariff on export; the import keeps it in its groups while the tariffs of a
  connector are as before, else attaches them to the EVSE.
  Operators are never created by the import.
- **Capabilities on import** are taken for new charging pools only; existing ones keep their authentication modes,
  payment options and features.
- **Status timestamps:** the importer follows OCPI's status, also where the history holds a later one (e.g. the default
  status of an EVSE just imported): the CommonAPI keeps an EVSE only with a later `last_updated`.
