///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

function StartLocations()
{

    function CreateLine(parent: HTMLDivElement | HTMLAnchorElement, className: string, key: string, innerHTML: string | HTMLDivElement): HTMLDivElement {

        const rowDiv = parent.appendChild(document.createElement('div')) as HTMLDivElement;
        rowDiv.className = "row";

        // key
        const keyDiv = rowDiv.appendChild(document.createElement('div')) as HTMLDivElement;
        keyDiv.className = "key";
        keyDiv.innerHTML = key;

        // value
        const valueDiv = rowDiv.appendChild(document.createElement('div')) as HTMLDivElement;
        valueDiv.className = "value " + className;

        if (typeof innerHTML === 'string')
            valueDiv.innerHTML = innerHTML;

        else if (innerHTML instanceof HTMLDivElement)
            valueDiv.appendChild(innerHTML);


        return rowDiv;

    }


    const locationInfosDiv             = document.getElementById("locationInfos")                     as HTMLDivElement;
    const locationsDiv                 = locationInfosDiv.querySelector("#locations")                 as HTMLDivElement;

    const numberOfLocationsDiv         = locationInfosDiv.querySelector("#numberOfLocations")         as HTMLDivElement;
    const numberOfEVSEsDiv             = locationInfosDiv.querySelector("#numberOfEVSEs")             as HTMLDivElement;
    const numberOfEVSEsAvailableDiv    = locationInfosDiv.querySelector("#numberOfEVSEsAvailable")    as HTMLDivElement;
    const numberOfEVSEsChargingDiv     = locationInfosDiv.querySelector("#numberOfEVSEsCharging")     as HTMLDivElement;
    const numberOfEVSEsOutOfOrderDiv   = locationInfosDiv.querySelector("#numberOfEVSEsOutOfOrder")   as HTMLDivElement;
    const numberOfEVSEsInoperativeDiv  = locationInfosDiv.querySelector("#numberOfEVSEsInoperative")  as HTMLDivElement;
    const numberOfEVSEsUnkownDiv       = locationInfosDiv.querySelector("#numberOfEVSEsUnkown")       as HTMLDivElement;

    let   totalNumberOfEVSEs           = 0;
    let   evsesAvailable               = 0;
    let   evsesCharging                = 0;
    let   evsesOutOfOrder              = 0;
    let   evsesInoperative             = 0;
    let   evsesUnkown                  = 0;
    let   evsesX                       = 0;

    OCPIGet(window.location.href,

            (status, response) => {

                try
                {

                    const OCPIResponse = ParseJSON_LD<IOCPIResponse>(response);

                    if (OCPIResponse?.data != undefined  &&
                        OCPIResponse?.data != null       &&
                        Array.isArray(OCPIResponse.data) &&
                        OCPIResponse.data.length > 0)
                    {

                        numberOfLocationsDiv.innerHTML = OCPIResponse.data.length.toString();

                        let numberOfLocation = 1;

                        for (const location of (OCPIResponse.data as ILocation[])) {

                            if (location.evses)
                                totalNumberOfEVSEs += location.evses.length;

                            const locationDiv           = locationsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            locationDiv.className       = "location";

                            const locationTitleDiv      = locationDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            locationTitleDiv.className  = "title";
                            locationTitleDiv.innerHTML  = (numberOfLocation++) + ". " + (location.name
                                                                                             ? location.name + " (" + location.id + ")"
                                                                                             : location.id);
                                                          // country_code
                                                          // party_id

                            const propertiesDiv         = locationDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            propertiesDiv.className     = "properties";

                            CreateLine(
                                propertiesDiv,
                                "details",
                                "Type",
                                location.location_type
                            )

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
                                CreateLine(
                                    propertiesDiv,
                                    "businessDetails",
                                    "Operator",
                                   (location.operator.website
                                        ? "<a href=\"" + location.operator.website + "\">" + location.operator.name + "</a>"
                                        : location.operator.name)
                                )
                            }

                            if (location.suboperator) {
                                CreateLine(
                                    propertiesDiv,
                                    "businessDetails",
                                    "Suboperator",
                                   (location.suboperator.website
                                        ? "<a href=\"" + location.suboperator.website + "\">" + location.suboperator.name + "</a>"
                                        : location.suboperator.name)
                                )
                            }

                            if (location.owner) {
                                CreateLine(
                                    propertiesDiv,
                                    "businessDetails",
                                    "Owner",
                                    (location.owner.website
                                        ? "<a href=\"" + location.owner.website + "\">" + location.owner.name + "</a>"
                                        : location.owner.name)
                                )
                            }

                            const evsesDiv        = locationDiv.appendChild(document.createElement('a')) as HTMLAnchorElement;
                            evsesDiv.className    = "evses";

                            let   numberOfEVSE    = 1;
                            const numberOfEVSEs   = location.evses?.length ?? 0;

                            if (location.evses) {
                                for (var evse of location.evses) {

                                    const evseDiv           = evsesDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                    evseDiv.className       = "evse evseStatus_" + evse.status;

                                    const statusDiv         = evseDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                    statusDiv.className     = "evseStatus evseStatus_" + evse.status;
                                    statusDiv.innerHTML     = evse.status;

                                    const evesIdDiv         = evseDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                    evesIdDiv.className     = "evseId";
                                    evesIdDiv.innerHTML     = (numberOfEVSE++) + ". " + evse.uid + (evse.evse_id && evse.evse_id != evse.uid ? " (" + evse.evse_id + ")" : "");
                                                              // physical_reference

                                    // capabilities
                                    // energy_meter (OCPI Calibration Law Extentions)
                                    // floor_level
                                    // coordinates
                                    // directions
                                    // parking_restrictions
                                    // images
                                    // last_updated

                                    const connectorsDiv     = evseDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                    connectorsDiv.className = "connectors";

                                    if (evse.connectors) {
                                        for (var connector of evse.connectors) {

                                            const connectorDiv     = connectorsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                            connectorDiv.className = "connector";

                                            const connectorInfoDiv = connectorDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                            connectorInfoDiv.className = "connectorInfo";
                                            connectorInfoDiv.innerHTML = connector.id + ". " + connector.standard + ", " + connector.format + ", " + connector.amperage + " A, " + connector.voltage + " V, " + connector.power_type;

                                            if (connector.tariff_id) {
                                                const tariffInfoDiv = connectorDiv.appendChild(document.createElement('div')) as HTMLDivElement;
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

                                        default:
                                            evsesX++;
                                            break;

                                    }

                                }
                            }

                        }

                        numberOfEVSEsDiv.innerHTML            = totalNumberOfEVSEs.toString();

                        numberOfEVSEsAvailableDiv.innerHTML   = evsesAvailable.toString();
                        numberOfEVSEsChargingDiv.innerHTML    = evsesCharging.toString();
                        numberOfEVSEsOutOfOrderDiv.innerHTML  = evsesOutOfOrder.toString();
                        numberOfEVSEsInoperativeDiv.innerHTML = evsesInoperative.toString();
                        numberOfEVSEsUnkownDiv.innerHTML      = evsesUnkown.toString();

                    }

                }
                catch (exception) {
                }

            },

            (status, statusText, response) => {
            }

    );

    //var refresh = setTimeout(StartDashboard, 30000);

}
