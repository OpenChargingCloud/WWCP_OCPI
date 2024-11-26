///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
function StartLocations() {
    function CreateLine(parent, className, key, innerHTML) {
        const rowDiv = parent.appendChild(document.createElement('div'));
        rowDiv.className = "row";
        // key
        const keyDiv = rowDiv.appendChild(document.createElement('div'));
        keyDiv.className = "key";
        keyDiv.innerHTML = key;
        // value
        const valueDiv = rowDiv.appendChild(document.createElement('div'));
        valueDiv.className = "value " + className;
        if (typeof innerHTML === 'string')
            valueDiv.innerHTML = innerHTML;
        else if (innerHTML instanceof HTMLDivElement)
            valueDiv.appendChild(innerHTML);
        return rowDiv;
    }
    const common = GetDefaults();
    common.topLeft.innerHTML = "/Locations";
    common.menuVersions.classList.add("activated");
    const locationInfosDiv = document.getElementById("locationInfos");
    const locationsDiv = locationInfosDiv.querySelector("#locations");
    const numberOfLocationsDiv = locationInfosDiv.querySelector("#numberOfLocations");
    const numberOfEVSEsDiv = locationInfosDiv.querySelector("#numberOfEVSEs");
    const numberOfEVSEsAvailableDiv = locationInfosDiv.querySelector("#numberOfEVSEsAvailable");
    const numberOfEVSEsChargingDiv = locationInfosDiv.querySelector("#numberOfEVSEsCharging");
    const numberOfEVSEsOutOfOrderDiv = locationInfosDiv.querySelector("#numberOfEVSEsOutOfOrder");
    const numberOfEVSEsInoperativeDiv = locationInfosDiv.querySelector("#numberOfEVSEsInoperative");
    const numberOfEVSEsUnkownDiv = locationInfosDiv.querySelector("#numberOfEVSEsUnkown");
    const numberOfEVSEsRemovedDiv = locationInfosDiv.querySelector("#numberOfEVSEsRemoved");
    const numberOfEVSEsXDiv = locationInfosDiv.querySelector("#numberOfEVSEsX");
    let totalNumberOfEVSEs = 0;
    let evsesAvailable = 0;
    let evsesCharging = 0;
    let evsesOutOfOrder = 0;
    let evsesInoperative = 0;
    let evsesUnkown = 0;
    let evsesRemoved = 0;
    let evsesX = 0;
    OCPIGet(window.location.href, (status, response) => {
        var _a, _b;
        try {
            const ocpiResponse = JSON.parse(response);
            if ((ocpiResponse === null || ocpiResponse === void 0 ? void 0 : ocpiResponse.data) != undefined &&
                (ocpiResponse === null || ocpiResponse === void 0 ? void 0 : ocpiResponse.data) != null &&
                Array.isArray(ocpiResponse.data) &&
                ocpiResponse.data.length > 0) {
                numberOfLocationsDiv.innerHTML = ocpiResponse.data.length.toString();
                let numberOfLocation = 1;
                for (const location of ocpiResponse.data) {
                    if (location.evses)
                        totalNumberOfEVSEs += location.evses.length;
                    const locationDiv = locationsDiv.appendChild(document.createElement('div'));
                    locationDiv.className = "location";
                    const locationTitleDiv = locationDiv.appendChild(document.createElement('div'));
                    locationTitleDiv.className = "title";
                    locationTitleDiv.innerHTML = (numberOfLocation++) + ". " + (location.name
                        ? location.name + " (" + location.id + ")"
                        : location.id);
                    // country_code
                    // party_id
                    const propertiesDiv = locationDiv.appendChild(document.createElement('div'));
                    propertiesDiv.className = "properties";
                    CreateLine(propertiesDiv, "details", "Type", location.type);
                    // address
                    // city
                    // postal_code
                    // country
                    // coordinates
                    // related_locations
                    // directions
                    // facilities
                    // time_zone
                    // opening_times
                    // charging_when_closed
                    // images
                    // energy_mix
                    // publish (PlugSurfing extension)
                    if (location.operator) {
                        CreateLine(propertiesDiv, "businessDetails", "Operator", (location.operator.website
                            ? "<a href=\"" + location.operator.website + "\">" + location.operator.name + "</a>"
                            : location.operator.name));
                    }
                    if (location.suboperator) {
                        CreateLine(propertiesDiv, "businessDetails", "Suboperator", (location.suboperator.website
                            ? "<a href=\"" + location.suboperator.website + "\">" + location.suboperator.name + "</a>"
                            : location.suboperator.name));
                    }
                    if (location.owner) {
                        CreateLine(propertiesDiv, "businessDetails", "Owner", (location.owner.website
                            ? "<a href=\"" + location.owner.website + "\">" + location.owner.name + "</a>"
                            : location.owner.name));
                    }
                    const evsesDiv = locationDiv.appendChild(document.createElement('a'));
                    evsesDiv.className = "evses";
                    let numberOfEVSE = 1;
                    const numberOfEVSEs = (_b = (_a = location.evses) === null || _a === void 0 ? void 0 : _a.length) !== null && _b !== void 0 ? _b : 0;
                    if (location.evses) {
                        for (var evse of location.evses) {
                            const evseDiv = evsesDiv.appendChild(document.createElement('div'));
                            evseDiv.className = "evse evseStatus_" + evse.status;
                            const statusDiv = evseDiv.appendChild(document.createElement('div'));
                            statusDiv.className = "evseStatus evseStatus_" + evse.status;
                            statusDiv.innerHTML = evse.status;
                            const evesIdDiv = evseDiv.appendChild(document.createElement('div'));
                            evesIdDiv.className = "evseId";
                            evesIdDiv.innerHTML = (numberOfEVSE++) + ". " + evse.uid + (evse.evse_id && evse.evse_id != evse.uid ? " (" + evse.evse_id + ")" : "");
                            // physical_reference
                            // capabilities
                            // energy_meter (OCPI Calibration Law Extentions)
                            // floor_level
                            // coordinates
                            // directions
                            // parking_restrictions
                            // images
                            // last_updated
                            const connectorsDiv = evseDiv.appendChild(document.createElement('div'));
                            connectorsDiv.className = "connectors";
                            if (evse.connectors) {
                                for (var connector of evse.connectors) {
                                    const connectorDiv = connectorsDiv.appendChild(document.createElement('div'));
                                    connectorDiv.className = "connector";
                                    const connectorInfoDiv = connectorDiv.appendChild(document.createElement('div'));
                                    connectorInfoDiv.className = "connectorInfo";
                                    connectorInfoDiv.innerHTML = connector.id + ". " + connector.standard + ", " + connector.format + ", " + connector.amperage + " A, " + connector.voltage + " V, " + connector.power_type;
                                    if (connector.tariff_id) {
                                        const tariffInfoDiv = connectorDiv.appendChild(document.createElement('div'));
                                        tariffInfoDiv.className = "tariffInfo";
                                        tariffInfoDiv.innerHTML = "Tariff: " + connector.tariff_id + (connector.terms_and_conditions
                                            ? ", terms: " + connector.terms_and_conditions
                                            : "");
                                    }
                                    // last_updated
                                }
                            }
                            switch (evse.status) {
                                case "AVAILABLE":
                                    evsesAvailable++;
                                    break;
                                case "CHARGING":
                                    evsesCharging++;
                                    break;
                                case "OUTOFORDER":
                                    evsesOutOfOrder++;
                                    break;
                                case "INOPERATIVE":
                                    evsesInoperative++;
                                    break;
                                case "UNKNOWN":
                                    evsesUnkown++;
                                    break;
                                case "REMOVED":
                                    evsesRemoved++;
                                    break;
                                default:
                                    evsesX++;
                                    break;
                            }
                        }
                    }
                }
                numberOfEVSEsDiv.innerHTML = totalNumberOfEVSEs.toString();
                numberOfEVSEsAvailableDiv.innerHTML = evsesAvailable.toString();
                numberOfEVSEsChargingDiv.innerHTML = evsesCharging.toString();
                numberOfEVSEsOutOfOrderDiv.innerHTML = evsesOutOfOrder.toString();
                numberOfEVSEsInoperativeDiv.innerHTML = evsesInoperative.toString();
                numberOfEVSEsUnkownDiv.innerHTML = evsesUnkown.toString();
                numberOfEVSEsRemovedDiv.innerHTML = evsesRemoved.toString();
                numberOfEVSEsXDiv.innerHTML = evsesX.toString();
            }
        }
        catch (exception) {
        }
    }, (status, statusText, response) => {
    });
    //var refresh = setTimeout(StartDashboard, 30000);
}
//# sourceMappingURL=locations.js.map