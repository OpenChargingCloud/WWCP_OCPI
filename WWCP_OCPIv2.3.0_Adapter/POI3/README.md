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
  The parties of the operators must be parties of the CommonAPI. Pushing to remote EMSPs is the CommonAPI's business:
  the publisher only keeps its store up to date, which EMSPs pull from.
- **Importer:** every location or tariff added or changed becomes a change set against the head: the charging pool or
  charging tariff as OCPI tells it, onto the one there is, with what only POI3 knows kept (see below). The difference is
  computed as a three-way merge whose left side is its base (`RoamingNetwork.TryMerge(head, head, desired)`), so only what
  changed is in the change set; nothing changed, nothing is committed. It is signed by the given delegate, since an
  operator owns its data and every change set must be signed by its owners, and committed. A removed location or tariff
  removes its charging pool or charging tariff, and an EVSE whose status is `REMOVED` its EVSE (the CommonAPI keeps such
  EVSEs by default). EVSE status goes into the runtime status of the history (`DataSource` "OCPI").
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
| `energyMix`: energy sources, supplier, product | `energy_mix` (`is_green_energy` when every source is green): Gas → `GAS`, Coal/StoneCoal → `COAL`, Oil/OtherFossil → `GENERAL_FOSSIL`, NuclearFision/NuclearFusion → `NUCLEAR`, Sun → `SOLAR`, Wind → `WIND`, Geothermal/Biogas/OtherGreen → `GENERAL_GREEN` (import: `GAS` → Gas, `COAL` → Coal, `GENERAL_FOSSIL` → OtherFossil, `NUCLEAR` → NuclearFision, `SOLAR` → Sun, `WIND` → Wind, `WATER`/`GENERAL_GREEN` → OtherGreen) |
| `hotline`, else the operator's | `help_phone` |
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

### Charging connector ↔ connector

| POI3 | OCPI connector |
|---|---|
| connector `type` | `standard` (the names are OCPI's) |
| — | `format` = `SOCKET` |
| EVSE `currentTypes`: DC, AC_ThreePhases, AC_OnePhase | `power_type`: `DC`, `AC_3_PHASE`, `AC_1_PHASE` |
| EVSE `maxVoltage`, else 230 V AC / 400 V DC | `max_voltage` |
| EVSE `maxCurrent`, else 16 A AC / 50 A DC | `max_amperage` |
| EVSE `maxPower` | `max_electric_power` |
| EVSE `chargingTariffIds` ∪ tariffs of the EVSE, charging station and charging pool groups it belongs to | `tariff_ids` |
| operator `termsAndConditionsURL` | `terms_and_conditions` |

Import: the EVSE takes the union of the current types of its connectors, the maximum of their voltages, amperages and
powers, and the union of their tariffs.

### Charging tariff ↔ tariff

| POI3 | OCPI tariff |
|---|---|
| `currency` | `currency` |
| — | `tax_included` = `N/A` |
| `name`, `description` | `tariff_alt_text` (import: into `name`) |
| `tariffURL` | `tariff_alt_url` |
| `energyMix` | `energy_mix` (energy sources as for locations) |
| element: price components, restrictions | `elements` |
| price component `ENERGY`, `TIME`, `PARKING_TIME`, `FLAT`: price, VAT, step size | `ENERGY` (per kWh, step in Wh), `TIME`/`PARKING_TIME` (per hour, step in s), `FLAT` (step 1) |
| restrictions: start/end time, start/end date, min/max energy (Wh), min/max current, min/max power (W), min/max duration (s), days of week | `start_time`, `end_time`, `start_date`, `end_date`, `min_kwh`/`max_kwh` (kWh), `min_current`/`max_current`, `min_power`/`max_power` (kW), `min_duration`/`max_duration`, `day_of_week` |

### What the import keeps

A location imported onto an existing charging pool sets only `name`, `address`, `geoLocation`, `timeZone`, `parkingType`,
`hotline`, `openingTimes`, `energyMix`, `images`, `facilities`, `arrivalInstructions`, `chargingWhenClosed` and
`relatedLocations`; an existing EVSE only `physicalReference`, `currentTypes`, `maxVoltage`, `maxCurrent`, `maxPower`,
`chargingConnectors`, `chargingTariffIds`, `images` and `geoLocation`. Everything else of POI3 stays as it is,
e.g. descriptions, data licenses, brands, grid connection, energy meters, authentication modes, features, accessibility,
EVSE charging modes and reservation duration. EVSEs no longer at OCPI, or `REMOVED`, are removed; new ones are added
(see above).

Of these, what OCPI tells as it would tell the existing charging pool, EVSE or charging tariff is kept as it is: the
existing one is mapped to OCPI and back, and only where that differs from what OCPI tells does OCPI's value win. So a
street keeps its house number apart, a city its language, an EVSE no coordinates of its own when it has those of its
station or pool, a tariff its name and description apart, and EVSEs the tariffs of their groups. What a POI3 network
publishes, imported back into it, commits nothing.

## Gaps

What one side has and the other does not, or not alike:

- **Charging stations:** OCPI 2.3.0 has none. On export a station's properties end up in its EVSEs (physical reference,
  coordinates, images, features as capabilities) or are lost (manufacturer, model, calibration and certification info,
  vehicle types, accessibility, max power, parking spaces, hotline, opening times, brands); on import EVSEs go into existing
  or one new station per location.
- **Connectors:** POI3 has no format (socket/cable), cable length or weight, per-connector voltage, amperage, power or tariffs,
  and no connector capabilities (ISO 15118 Plug & Charge): `format` is always `SOCKET`, the rest comes from the EVSE.
  `lockable` has no OCPI counterpart. A plug type OCPI does not know leaves the connector out (with a warning).
- **Location:** POI3 has no `publish`/`publish_allowed_to` (always published), no `owner`, no parking places (the OCPI 2.3
  `Parking` objects vs. POI3 parking garages and spaces), no energy meters of locations, no accessibility extension
  (`services`, `assistance_service_details`, `standards`). The house number is joined with the street, and only split
  again where the address is as before (see above).
  Environmental impacts of energy mixes are not mapped (OCPI: g/kWh, POI3: in a percentage).
- **Energy sources:** OCPI has fewer categories than POI3: stone coal, oil, nuclear fusion, geothermal and biogas come back
  as Coal, OtherFossil, NuclearFision and OtherGreen. POI3 has no water, which becomes OtherGreen; OCPI has no waste, which
  is left out (with a warning), as are unknown sources.
- **POI3 only:** descriptions, entrance/exit addresses and locations, location languages, payment options and authentication
  modes beyond the capabilities above, pool features, grid connection points, data licenses, groups, brands (beyond the
  first as sub-operator), energy status of EVSEs and stations, charging pool and station status.
- **EVSE:** POI3 has no floor level, `status_schedule`, EVSE `parking` references, `accepted_service_providers` or the
  accessibility fields (`reach_distance`, `operation_timeout`); OCPI has no charging modes, reservation duration,
  names or descriptions of EVSEs.
- **Tariffs:** the POI3 price dimensions `MIN_CURRENT`/`MAX_CURRENT` have no OCPI 2.3.0 counterpart (such a tariff is not
  published); OCPI's `type`, `min_price`, `max_price`, `start_date_time`, `end_date_time`, the reservation restriction and
  `tax_included` have no POI3 counterpart (left out with a warning).
- **Payment terminals** (the OCPI 2.3 Payments module) are not mapped.
- **Identifications:** an OCPI party must have three characters to be an operator; a location id must fit a charging pool
  suffix (`[A-Za-z0-9][A-Za-z0-9*]{0,29}`) and a tariff id a charging tariff suffix; others are refused with a warning.
- **Import of groups and operators:** tariffs of EVSE, station or pool groups become connector tariffs on export; the
  import keeps them in their groups while the tariffs of a connector are as before, else attaches them to the EVSE.
  Operators are never created by the import.
- **Capabilities on import** are taken for new charging pools only; existing ones keep their authentication modes,
  payment options and features.
- **Status timestamps:** the importer follows OCPI's status, also where the history holds a later one (e.g. the default
  status of an EVSE just imported): the CommonAPI keeps an EVSE only with a later `last_updated`.
