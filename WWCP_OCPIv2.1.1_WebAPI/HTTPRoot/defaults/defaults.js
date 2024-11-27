// https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/
var Capability;
(function (Capability) {
    Capability[Capability["CHARGING_PROFILE_CAPABLE"] = 0] = "CHARGING_PROFILE_CAPABLE";
    Capability[Capability["CREDIT_CARD_PAYABLE"] = 1] = "CREDIT_CARD_PAYABLE";
    Capability[Capability["REMOTE_START_STOP_CAPABLE"] = 2] = "REMOTE_START_STOP_CAPABLE";
    Capability[Capability["RESERVABLE"] = 3] = "RESERVABLE";
    Capability[Capability["RFID_READER"] = 4] = "RFID_READER";
    Capability[Capability["UNLOCK_CAPABLE"] = 5] = "UNLOCK_CAPABLE"; // Connectors have mechanical lock that can be requested by the eMSP to be unlocked.
})(Capability || (Capability = {}));
var ConnectorFormat;
(function (ConnectorFormat) {
    ConnectorFormat[ConnectorFormat["SOCKET"] = 0] = "SOCKET";
    ConnectorFormat[ConnectorFormat["CABLE"] = 1] = "CABLE"; // The connector is an attached cable; the EV users car needs to have a fitting inlet.
})(ConnectorFormat || (ConnectorFormat = {}));
var ConnectorType;
(function (ConnectorType) {
    ConnectorType[ConnectorType["CHADEMO"] = 0] = "CHADEMO";
    ConnectorType[ConnectorType["DOMESTIC_A"] = 1] = "DOMESTIC_A";
    ConnectorType[ConnectorType["DOMESTIC_B"] = 2] = "DOMESTIC_B";
    ConnectorType[ConnectorType["DOMESTIC_C"] = 3] = "DOMESTIC_C";
    ConnectorType[ConnectorType["DOMESTIC_D"] = 4] = "DOMESTIC_D";
    ConnectorType[ConnectorType["DOMESTIC_E"] = 5] = "DOMESTIC_E";
    ConnectorType[ConnectorType["DOMESTIC_F"] = 6] = "DOMESTIC_F";
    ConnectorType[ConnectorType["DOMESTIC_G"] = 7] = "DOMESTIC_G";
    ConnectorType[ConnectorType["DOMESTIC_H"] = 8] = "DOMESTIC_H";
    ConnectorType[ConnectorType["DOMESTIC_I"] = 9] = "DOMESTIC_I";
    ConnectorType[ConnectorType["DOMESTIC_J"] = 10] = "DOMESTIC_J";
    ConnectorType[ConnectorType["DOMESTIC_K"] = 11] = "DOMESTIC_K";
    ConnectorType[ConnectorType["DOMESTIC_L"] = 12] = "DOMESTIC_L";
    ConnectorType[ConnectorType["IEC_60309_2_single_16"] = 13] = "IEC_60309_2_single_16";
    ConnectorType[ConnectorType["IEC_60309_2_three_16"] = 14] = "IEC_60309_2_three_16";
    ConnectorType[ConnectorType["IEC_60309_2_three_32"] = 15] = "IEC_60309_2_three_32";
    ConnectorType[ConnectorType["IEC_60309_2_three_64"] = 16] = "IEC_60309_2_three_64";
    ConnectorType[ConnectorType["IEC_62196_T1"] = 17] = "IEC_62196_T1";
    ConnectorType[ConnectorType["IEC_62196_T1_COMBO"] = 18] = "IEC_62196_T1_COMBO";
    ConnectorType[ConnectorType["IEC_62196_T2"] = 19] = "IEC_62196_T2";
    ConnectorType[ConnectorType["IEC_62196_T2_COMBO"] = 20] = "IEC_62196_T2_COMBO";
    ConnectorType[ConnectorType["IEC_62196_T3A"] = 21] = "IEC_62196_T3A";
    ConnectorType[ConnectorType["IEC_62196_T3C"] = 22] = "IEC_62196_T3C";
    ConnectorType[ConnectorType["TESLA_R"] = 23] = "TESLA_R";
    ConnectorType[ConnectorType["TESLA_S"] = 24] = "TESLA_S"; // Tesla Connector "Model-S" - type(oval, 5 pin)
})(ConnectorType || (ConnectorType = {}));
var EnergySourceCategory;
(function (EnergySourceCategory) {
    EnergySourceCategory[EnergySourceCategory["NUCLEAR"] = 0] = "NUCLEAR";
    EnergySourceCategory[EnergySourceCategory["GENERAL_FOSSIL"] = 1] = "GENERAL_FOSSIL";
    EnergySourceCategory[EnergySourceCategory["COAL"] = 2] = "COAL";
    EnergySourceCategory[EnergySourceCategory["GAS"] = 3] = "GAS";
    EnergySourceCategory[EnergySourceCategory["GENERAL_GREEN"] = 4] = "GENERAL_GREEN";
    EnergySourceCategory[EnergySourceCategory["SOLAR"] = 5] = "SOLAR";
    EnergySourceCategory[EnergySourceCategory["WIND"] = 6] = "WIND";
    EnergySourceCategory[EnergySourceCategory["WATER"] = 7] = "WATER"; // Regenerative power from water turbines.
})(EnergySourceCategory || (EnergySourceCategory = {}));
var EnvironmentalImpactCategory;
(function (EnvironmentalImpactCategory) {
    EnvironmentalImpactCategory[EnvironmentalImpactCategory["NUCLEAR_WASTE"] = 0] = "NUCLEAR_WASTE";
    EnvironmentalImpactCategory[EnvironmentalImpactCategory["CARBON_DIOXIDE"] = 1] = "CARBON_DIOXIDE"; // Exhausted carbon dioxide in gramms per kilowarrhour.
})(EnvironmentalImpactCategory || (EnvironmentalImpactCategory = {}));
var Facility;
(function (Facility) {
    Facility[Facility["HOTEL"] = 0] = "HOTEL";
    Facility[Facility["RESTAURANT"] = 1] = "RESTAURANT";
    Facility[Facility["CAFE"] = 2] = "CAFE";
    Facility[Facility["MALL"] = 3] = "MALL";
    Facility[Facility["SUPERMARKET"] = 4] = "SUPERMARKET";
    Facility[Facility["SPORT"] = 5] = "SPORT";
    Facility[Facility["RECREATION_AREA"] = 6] = "RECREATION_AREA";
    Facility[Facility["NATURE"] = 7] = "NATURE";
    Facility[Facility["MUSEUM"] = 8] = "MUSEUM";
    Facility[Facility["BUS_STOP"] = 9] = "BUS_STOP";
    Facility[Facility["TAXI_STAND"] = 10] = "TAXI_STAND";
    Facility[Facility["TRAIN_STATION"] = 11] = "TRAIN_STATION";
    Facility[Facility["AIRPORT"] = 12] = "AIRPORT";
    Facility[Facility["CARPOOL_PARKING"] = 13] = "CARPOOL_PARKING";
    Facility[Facility["FUEL_STATION"] = 14] = "FUEL_STATION";
    Facility[Facility["WIFI"] = 15] = "WIFI"; // Wifi or other type of internet available.
})(Facility || (Facility = {}));
var ImageCategory;
(function (ImageCategory) {
    ImageCategory[ImageCategory["CHARGER"] = 0] = "CHARGER";
    ImageCategory[ImageCategory["ENTRANCE"] = 1] = "ENTRANCE";
    ImageCategory[ImageCategory["LOCATION"] = 2] = "LOCATION";
    ImageCategory[ImageCategory["NETWORK"] = 3] = "NETWORK";
    ImageCategory[ImageCategory["OPERATOR"] = 4] = "OPERATOR";
    ImageCategory[ImageCategory["OTHER"] = 5] = "OTHER";
    ImageCategory[ImageCategory["OWNER"] = 6] = "OWNER"; // logo of the charge points owner, for example a local store, to be displayed with the EVSEs detailed information view
})(ImageCategory || (ImageCategory = {}));
var LocationType;
(function (LocationType) {
    LocationType[LocationType["ON_STREET"] = 0] = "ON_STREET";
    LocationType[LocationType["PARKING_GARAGE"] = 1] = "PARKING_GARAGE";
    LocationType[LocationType["UNDERGROUND_GARAGE"] = 2] = "UNDERGROUND_GARAGE";
    LocationType[LocationType["PARKING_LOT"] = 3] = "PARKING_LOT";
    LocationType[LocationType["OTHER"] = 4] = "OTHER";
    LocationType[LocationType["UNKNOWN"] = 5] = "UNKNOWN"; // Parking location type is not known by the operator (default).
})(LocationType || (LocationType = {}));
var ParkingRestriction;
(function (ParkingRestriction) {
    ParkingRestriction[ParkingRestriction["EV_ONLY"] = 0] = "EV_ONLY";
    ParkingRestriction[ParkingRestriction["PLUGGED"] = 1] = "PLUGGED";
    ParkingRestriction[ParkingRestriction["DISABLED"] = 2] = "DISABLED";
    ParkingRestriction[ParkingRestriction["CUSTOMERS"] = 3] = "CUSTOMERS";
    ParkingRestriction[ParkingRestriction["MOTORCYCLES"] = 4] = "MOTORCYCLES"; // Parking spot only suitable for (electric) motorcycles or scooters.
})(ParkingRestriction || (ParkingRestriction = {}));
var PowerType;
(function (PowerType) {
    PowerType[PowerType["AC_1_PHASE"] = 0] = "AC_1_PHASE";
    PowerType[PowerType["AC_3_PHASE"] = 1] = "AC_3_PHASE";
    PowerType[PowerType["DC"] = 2] = "DC"; // Direct Current.
})(PowerType || (PowerType = {}));
var Status;
(function (Status) {
    Status[Status["AVAILABLE"] = 0] = "AVAILABLE";
    Status[Status["BLOCKED"] = 1] = "BLOCKED";
    Status[Status["CHARGING"] = 2] = "CHARGING";
    Status[Status["INOPERATIVE"] = 3] = "INOPERATIVE";
    Status[Status["OUTOFORDER"] = 4] = "OUTOFORDER";
    Status[Status["PLANNED"] = 5] = "PLANNED";
    Status[Status["REMOVED"] = 6] = "REMOVED";
    Status[Status["RESERVED"] = 7] = "RESERVED";
    Status[Status["UNKNOWN"] = 8] = "UNKNOWN"; // No status information available. (Also used when offline)
})(Status || (Status = {}));
var TariffDimension;
(function (TariffDimension) {
    TariffDimension[TariffDimension["ENERGY"] = 0] = "ENERGY";
    TariffDimension[TariffDimension["FLAT"] = 1] = "FLAT";
    TariffDimension[TariffDimension["PARKING_TIME"] = 2] = "PARKING_TIME";
    TariffDimension[TariffDimension["TIME"] = 3] = "TIME"; // time charging: defined in hours, step_size multiplier: 1 second
})(TariffDimension || (TariffDimension = {}));
var DayOfWeek;
(function (DayOfWeek) {
    DayOfWeek[DayOfWeek["MONDAY"] = 0] = "MONDAY";
    DayOfWeek[DayOfWeek["TUESDAY"] = 1] = "TUESDAY";
    DayOfWeek[DayOfWeek["WEDNESDAY"] = 2] = "WEDNESDAY";
    DayOfWeek[DayOfWeek["THURSDAY"] = 3] = "THURSDAY";
    DayOfWeek[DayOfWeek["FRIDAY"] = 4] = "FRIDAY";
    DayOfWeek[DayOfWeek["SATURDAY"] = 5] = "SATURDAY";
    DayOfWeek[DayOfWeek["SUNDAY"] = 6] = "SUNDAY";
})(DayOfWeek || (DayOfWeek = {}));
let topLeft = null;
function GetDefaults() {
    return {
        topLeft: document.getElementById("topLeft"),
        menuVersions: document.getElementById("menuVersions")
    };
}
function EncodeToken(str) {
    var buf = [];
    for (let i = str.length - 1; i >= 0; i--) {
        buf.unshift(['&#', str[i].charCodeAt(), ';'].join(''));
    }
    return buf.join('');
}
// #region OCPIGet(RessourceURI, AccessToken, OnSuccess, OnError)
function OCPIGet(RessourceURI, OnSuccess, OnError) {
    const ajax = new XMLHttpRequest();
    ajax.open("GET", RessourceURI, true);
    ajax.setRequestHeader("Accept", "application/json; charset=UTF-8");
    ajax.setRequestHeader("X-Portal", "true");
    if (localStorage.getItem("OCPIAccessToken") != null)
        ajax.setRequestHeader("Authorization", "Token " + btoa(localStorage.getItem("OCPIAccessToken")));
    ajax.onreadystatechange = function () {
        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {
            // Ok
            if (this.status >= 100 && this.status < 300) {
                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));
                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(this.status, ajax.responseText);
            }
            else if (OnError && typeof OnError === 'function')
                OnError(this.status, this.statusText, ajax.responseText);
        }
    };
    ajax.send();
}
// #endregion
//# sourceMappingURL=defaults.js.map