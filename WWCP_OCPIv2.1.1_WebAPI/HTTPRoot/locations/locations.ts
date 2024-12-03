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


    const common                       = GetDefaults();
    common.topLeft.innerHTML           = "/Locations"
    common.menuVersions.classList.add("activated");
    common.menuVersions.href           = "../../versions";

    const tariffs                      = new Map<string, number>();
    const errors                       = new Array<any>();

    OCPIStartSearch2<ILocationMetadata, ILocation>(

        window.location.href,
        () => {
            //return (statusFilterSelect.selectedOptions[0].value !== "any" ? "&matchStatus=" + statusFilterSelect.selectedOptions[0].value : "");
            return "";
        },
        metadata => {

            //if (metadata["description"] != null && firstValue(metadata["description"]) != null)
            //{
            //    (communicatorDescription.querySelector("#language") as HTMLDivElement).innerText = firstKey  (metadata["description"]);
            //    (communicatorDescription.querySelector("#I18NText") as HTMLDivElement).innerText = firstValue(metadata["description"]);
            //}

        },
        "location",
        location => location.id,
        "locations",
        "locations",

        // list view
        (resultCounter,
         location,
         locationAnchor) => {

            //if (location.evses)
            //    totalNumberOfEVSEs += location.evses.length;

            const locationCounterDiv      = locationAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            locationCounterDiv.className  = "counter";
            locationCounterDiv.innerHTML  = resultCounter.toString() + ".";

            const locationTitleDiv        = locationAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            locationTitleDiv.className    = "title";
            locationTitleDiv.innerHTML    = location.name
                                                ? location.name + " (" + location.id + ")"
                                                : location.id;
                                            // country_code
                                            // party_id

            const propertiesDiv           = locationAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            propertiesDiv.className       = "properties";

            if (location.operator) {
                CreateLine(
                    propertiesDiv,
                    "businessDetails operator",
                    "Operator",
                   (location.operator.website
                        ? "<a href=\"" + location.operator.website + "\">" + location.operator.name + "</a>"
                        : location.operator.name)
                )
            }

            if (location.suboperator) {
                 CreateLine(
                     propertiesDiv,
                     "businessDetails suboperator",
                     "Suboperator",
                    (location.suboperator.website
                         ? "<a href=\"" + location.suboperator.website + "\">" + location.suboperator.name + "</a>"
                         : location.suboperator.name)
                 )
             }

            if (location.owner) {
                 CreateLine(
                     propertiesDiv,
                     "businessDetails owner",
                     "Owner",
                     (location.owner.website
                         ? "<a href=\"" + location.owner.website + "\">" + location.owner.name + "</a>"
                         : location.owner.name)
                 )
             }

            CreateLine(
                propertiesDiv,
                "type",
                "Type",
                location.type
            )

            CreateLine(
                propertiesDiv,
                "address",
                "Address",
                location.address + ", " + location.postal_code + " " + location.city + ", " + location.country + (location.time_zone ? " (" + location.time_zone + ")" : "")
            );

            CreateLine(
                propertiesDiv,
                "coordinates",
                "Lat/Lng",
                location.coordinates.latitude + ", " + location.coordinates.longitude
            );

            // opening_times?

            // related_locations?
            // directions?
            // facilities?

            // charging_when_closed?
            // images?
            // energy_mix?

            // publish (PlugSurfing extension)

            const evsesDiv        = locationAnchor.appendChild(document.createElement('a')) as HTMLAnchorElement;
            evsesDiv.className    = "evses";

            let   numberOfEVSE    = 1;
          //  const numberOfEVSEs   = location.evses?.length ?? 0;

            if (location.evses) {
                for (var evse of location.evses) {

                    const evseDiv                = evsesDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    evseDiv.className            = "evse evseStatus_" + evse.status;

                    const statusDiv              = evseDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    statusDiv.className          = "evseStatus evseStatus_" + evse.status;
                    statusDiv.innerHTML          = evse.status;

                    const evesIdDiv              = evseDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    evesIdDiv.className          = "evseId";
                    evesIdDiv.innerHTML          = (numberOfEVSE++) + ". " + evse.uid + (evse.evse_id && evse.evse_id != evse.uid ? " (" + evse.evse_id + ")" : "")
                                                       + (evse.physical_reference ? " [" + evse.physical_reference + "]" : "");

                    const evsePropertiesDiv      = evseDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    evsePropertiesDiv.className  = "properties";

                    if (evse.floor_level)
                        CreateLine(
                            evsePropertiesDiv,
                            "floorLevel",
                            "Floor Level",
                            evse.floor_level
                        );

                    CreateLine(
                        evsePropertiesDiv,
                        "coordinates",
                        "Lat/Lng",
                        evse.coordinates.latitude + ", " + evse.coordinates.longitude
                    );

                    if (evse.capabilities)
                        CreateLine(
                            evsePropertiesDiv,
                            "capabilities",
                            "Capabilities",
                            evse.capabilities.map(capability => capability).join(", ")
                        );

                    if (evse.parking_restrictions)
                        CreateLine(
                            evsePropertiesDiv,
                            "parkingRestrictions",
                            "Parking Restrictions",
                            evse.parking_restrictions.map(parkingRestriction => parkingRestriction).join(", ")
                        );

                    if (evse.images)
                        CreateLine(
                            evsePropertiesDiv,
                            "images",
                            "Images",
                            evse.images.map(image => image).join("<br />")
                        );

                    if (evse.directions)
                        CreateLine(
                            evsePropertiesDiv,
                            "directions",
                            "Directions",
                            evse.directions.map(direction => "(" + direction.language + ") " + direction.text).join("<br />")
                        );

                    if (evse.status_schedule)
                        CreateLine(
                            evsePropertiesDiv,
                            "statusSchedule",
                            "Status Schedule",
                            evse.status_schedule.map(statusSchedule => statusSchedule.status + " (" + statusSchedule.period_begin + (statusSchedule.period_end ? " => " + statusSchedule.period_end : "") + ")").join("<br />")
                        );

                    // energy_meter (OCC Calibration Law Extentions)

                    const connectorsDiv     = evseDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    connectorsDiv.className = "connectors";

                    if (evse.connectors) {
                        for (var connector of evse.connectors) {

                            const connectorDiv                = connectorsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            connectorDiv.className            = "connector";

                            const connectorInfoDiv            = connectorDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            connectorInfoDiv.className        = "connectorInfo";
                            connectorInfoDiv.innerHTML        = connector.id + ". " + connector.standard + ", " + connector.format + ", " + connector.amperage + " A, " + connector.voltage + " V, " + connector.power_type;

                            const connectorPropertiesDiv      = connectorDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            connectorPropertiesDiv.className  = "properties";

                            if (connector.tariff_id) {

                                if (!tariffs.has(connector.tariff_id)) {
                                    tariffs.set(connector.tariff_id, 0);
                                }

                                tariffs.set(
                                    connector.tariff_id,
                                    tariffs.get(connector.tariff_id) + 1
                                );

                                CreateLine(
                                    connectorPropertiesDiv,
                                    "tariffInfo",
                                    "Tariff",
                                    connector.tariff_id
                                );

                            }

                            if (connector.terms_and_conditions)
                                CreateLine(
                                    connectorPropertiesDiv,
                                    "terms",
                                    "Terms",
                                    connector.terms_and_conditions
                                );

                            const connectorLastUpdatedDiv = connectorDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            connectorLastUpdatedDiv.className = "lastUpdated";
                            connectorLastUpdatedDiv.innerHTML = "Last updated: " + connector.last_updated;

                        }
                    }

                    const evseLastUpdatedDiv = evseDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    evseLastUpdatedDiv.className = "lastUpdated";
                    evseLastUpdatedDiv.innerHTML = "Last updated: " + evse.last_updated;

                }
            }

            const locationLastUpdatedDiv = locationAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            locationLastUpdatedDiv.className = "lastUpdated";
            locationLastUpdatedDiv.innerHTML = "Last updated: " + location.last_updated;

        },

        // table view
        (tariffs, tariffsDiv) => {
        },

        // linkPrefix
        null,//tariff => "",
        searchResultsMode.listView,

        context => {
            //statusFilterSelect.onchange = () => {
            //    context.Search(true);
            //}
        }

    );

}




    //OCPIGet(window.location.href,

    //        (status, response) => {

    //            try
    //            {

    //                const ocpiResponse = JSON.parse(response) as IOCPIResponse;

    //                if (ocpiResponse?.data != undefined  &&
    //                    ocpiResponse?.data != null       &&
    //                    Array.isArray(ocpiResponse.data) &&
    //                    ocpiResponse.data.length > 0)
    //                {

    //                    numberOfLocationsDiv.innerHTML = ocpiResponse.data.length.toString();

    //                    let numberOfLocation = 1;

    //                    for (const location of (ocpiResponse.data as ILocation[])) {

    //                        if (location.evses)
    //                            totalNumberOfEVSEs += location.evses.length;

    //                        const locationDiv           = locationsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    //                        locationDiv.className       = "location";

    //                        const locationTitleDiv      = locationDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    //                        locationTitleDiv.className  = "title";
    //                        locationTitleDiv.innerHTML  = (numberOfLocation++) + ". " + (location.name
    //                                                                                         ? location.name + " (" + location.id + ")"
    //                                                                                         : location.id);
    //                                                      // country_code
    //                                                      // party_id

    //                        const propertiesDiv         = locationDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    //                        propertiesDiv.className     = "properties";

    //                        CreateLine(
    //                            propertiesDiv,
    //                            "details",
    //                            "Type",
    //                            location.type
    //                        )

    //                        // address
    //                        // city
    //                        // postal_code
    //                        // country
    //                        // coordinates
    //                        // related_locations
    //                        // directions
    //                        // facilities
    //                        // time_zone
    //                        // opening_times
    //                        // charging_when_closed
    //                        // images
    //                        // energy_mix
    //                        // publish (PlugSurfing extension)

    //                        if (location.operator) {
    //                            CreateLine(
    //                                propertiesDiv,
    //                                "businessDetails",
    //                                "Operator",
    //                               (location.operator.website
    //                                    ? "<a href=\"" + location.operator.website + "\">" + location.operator.name + "</a>"
    //                                    : location.operator.name)
    //                            )
    //                        }

    //                        if (location.suboperator) {
    //                            CreateLine(
    //                                propertiesDiv,
    //                                "businessDetails",
    //                                "Suboperator",
    //                               (location.suboperator.website
    //                                    ? "<a href=\"" + location.suboperator.website + "\">" + location.suboperator.name + "</a>"
    //                                    : location.suboperator.name)
    //                            )
    //                        }

    //                        if (location.owner) {
    //                            CreateLine(
    //                                propertiesDiv,
    //                                "businessDetails",
    //                                "Owner",
    //                                (location.owner.website
    //                                    ? "<a href=\"" + location.owner.website + "\">" + location.owner.name + "</a>"
    //                                    : location.owner.name)
    //                            )
    //                        }

    //                        const evsesDiv        = locationDiv.appendChild(document.createElement('a')) as HTMLAnchorElement;
    //                        evsesDiv.className    = "evses";

    //                        let   numberOfEVSE    = 1;
    //                        const numberOfEVSEs   = location.evses?.length ?? 0;

    //                        if (location.evses) {
    //                            for (var evse of location.evses) {

    //                                const evseDiv           = evsesDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    //                                evseDiv.className       = "evse evseStatus_" + evse.status;

    //                                const statusDiv         = evseDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    //                                statusDiv.className     = "evseStatus evseStatus_" + evse.status;
    //                                statusDiv.innerHTML     = evse.status;

    //                                const evesIdDiv         = evseDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    //                                evesIdDiv.className     = "evseId";
    //                                evesIdDiv.innerHTML     = (numberOfEVSE++) + ". " + evse.uid + (evse.evse_id && evse.evse_id != evse.uid ? " (" + evse.evse_id + ")" : "");
    //                                                          // physical_reference

    //                                // capabilities
    //                                // energy_meter (OCPI Calibration Law Extentions)
    //                                // floor_level
    //                                // coordinates
    //                                // directions
    //                                // parking_restrictions
    //                                // images
    //                                // last_updated

    //                                const connectorsDiv     = evseDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    //                                connectorsDiv.className = "connectors";

    //                                if (evse.connectors) {
    //                                    for (var connector of evse.connectors) {

    //                                        const connectorDiv     = connectorsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    //                                        connectorDiv.className = "connector";

    //                                        const connectorInfoDiv = connectorDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    //                                        connectorInfoDiv.className = "connectorInfo";
    //                                        connectorInfoDiv.innerHTML = connector.id + ". " + connector.standard + ", " + connector.format + ", " + connector.amperage + " A, " + connector.voltage + " V, " + connector.power_type;

    //                                        if (connector.tariff_id) {

    //                                            if (!tariffs.has(connector.tariff_id)) {
    //                                                tariffs.set(connector.tariff_id, 0);
    //                                            }

    //                                            tariffs.set(
    //                                                connector.tariff_id,
    //                                                tariffs.get(connector.tariff_id) + 1
    //                                            );

    //                                            const tariffInfoDiv = connectorDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    //                                            tariffInfoDiv.className = "tariffInfo";
    //                                            tariffInfoDiv.innerHTML = "Tariff: " + connector.tariff_id + (connector.terms_and_conditions
    //                                                                                                              ? ", terms: " + connector.terms_and_conditions
    //                                                                                                              : "");

    //                                        }

    //                                        // last_updated

    //                                    }
    //                                }

    //                                switch (evse.status) {

    //                                    case "AVAILABLE":
    //                                        evsesAvailable++;
    //                                        break;

    //                                    case "CHARGING":
    //                                        evsesCharging++;
    //                                        break;

    //                                    case "OUTOFORDER":
    //                                        evsesOutOfOrder++;
    //                                        break;

    //                                    case "INOPERATIVE":
    //                                        evsesInoperative++;
    //                                        break;

    //                                    case "UNKNOWN":
    //                                        evsesUnkown++;
    //                                        break;

    //                                    case "REMOVED":
    //                                        evsesRemoved++;
    //                                        break;

    //                                    default:
    //                                        evsesX++;
    //                                        break;

    //                                }

    //                            }
    //                        }

    //                    }

    //                    numberOfEVSEsDiv.innerHTML            = totalNumberOfEVSEs.toString();

    //                    numberOfEVSEsAvailableDiv.innerHTML   = evsesAvailable.toString();
    //                    numberOfEVSEsChargingDiv.innerHTML    = evsesCharging.toString();
    //                    numberOfEVSEsOutOfOrderDiv.innerHTML  = evsesOutOfOrder.toString();
    //                    numberOfEVSEsInoperativeDiv.innerHTML = evsesInoperative.toString();
    //                    numberOfEVSEsUnkownDiv.innerHTML      = evsesUnkown.toString();
    //                    numberOfEVSEsRemovedDiv.innerHTML     = evsesRemoved.toString();
    //                    numberOfEVSEsXDiv.innerHTML           = evsesX.toString();


    //                    for (const [tariffId, count] of tariffs) {
    //                        CreateLine(
    //                            tariffStatisticsDiv,
    //                            "tariff",
    //                            tariffId,
    //                            count.toString()
    //                        )
    //                    }

    //                }

    //            }
    //            catch (exception) {
    //                errors.push(exception);
    //            }

    //        },

    //        (status, statusText, response) => {
    //        }

    //);

    //var refresh = setTimeout(StartDashboard, 30000);

//}
