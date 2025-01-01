/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

function StartLocations()
{

    const common                       = GetDefaults();
    common.topLeft.innerHTML           = "/Locations"
    common.menuVersions.classList.add("activated");
    common.menuVersions.href           = "../../versions";

    //const tariffs                      = new Map<string, number>();
    //const errors                       = new Array<any>();

    OCPIStartSearch2<ILocationMetadata, ILocation>(

        window.location.href,
        () => {
            return "";
        },
        metadata => { },
        "location",
        location => location.id,
        "locations",
        "locations",

        // list view
        (resultCounter,
         location,
         locationAnchor) => {

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
                CreateProperty(
                    propertiesDiv,
                    "businessDetails operator",
                    "Operator",
                   (location.operator.website
                        ? "<a href=\"" + location.operator.website + "\">" + location.operator.name + "</a>"
                        : location.operator.name)
                )
            }

            if (location.suboperator) {
                 CreateProperty(
                     propertiesDiv,
                     "businessDetails suboperator",
                     "Suboperator",
                    (location.suboperator.website
                         ? "<a href=\"" + location.suboperator.website + "\">" + location.suboperator.name + "</a>"
                         : location.suboperator.name)
                 )
             }

            if (location.owner) {
                 CreateProperty(
                     propertiesDiv,
                     "businessDetails owner",
                     "Owner",
                     (location.owner.website
                         ? "<a href=\"" + location.owner.website + "\">" + location.owner.name + "</a>"
                         : location.owner.name)
                 )
             }

            CreateProperty(
                propertiesDiv,
                "parkingType",
                "Parking Type",
                location.parking_type
            )

            CreateProperty(
                propertiesDiv,
                "address",
                "Address",
                location.address + ", " + location.postal_code + " " + location.city + ", " + location.country + (location.time_zone ? " (" + location.time_zone + ")" : "")
            );

            CreateProperty(
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

            let numberOfEVSE = 1;

            if (location.evses) {
                for (const evse of location.evses) {

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
                        CreateProperty(
                            evsePropertiesDiv,
                            "floorLevel",
                            "Floor Level",
                            evse.floor_level
                        );

                    if (evse.coordinates)
                        CreateProperty(
                            evsePropertiesDiv,
                            "coordinates",
                            "Lat/Lng",
                            evse.coordinates.latitude + ", " + evse.coordinates.longitude
                        );

                    if (evse.capabilities)
                        CreateProperty(
                            evsePropertiesDiv,
                            "capabilities",
                            "Capabilities",
                            evse.capabilities.map(capability => capability).join(", ")
                        );

                    if (evse.parking_restrictions)
                        CreateProperty(
                            evsePropertiesDiv,
                            "parkingRestrictions",
                            "Parking Restrictions",
                            evse.parking_restrictions.map(parkingRestriction => parkingRestriction).join(", ")
                        );

                    if (evse.images)
                        CreateProperty(
                            evsePropertiesDiv,
                            "images",
                            "Images",
                            evse.images.map(image => image).join("<br />")
                        );

                    if (evse.directions)
                        CreateProperty(
                            evsePropertiesDiv,
                            "directions",
                            "Directions",
                            evse.directions.map(direction => "(" + direction.language + ") " + direction.text).join("<br />")
                        );

                    if (evse.status_schedule)
                        CreateProperty(
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
                            connectorInfoDiv.innerHTML        = connector.id + ". " + connector.standard + ", " + connector.format + ", " + connector.max_amperage + " A, " + connector.max_voltage + " V, " + connector.power_type;

                            const connectorPropertiesDiv      = connectorDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            connectorPropertiesDiv.className  = "properties";

                            if (connector.tariff_ids)
                                CreateProperty(
                                    connectorPropertiesDiv,
                                    "tariffsInfo",
                                    "Tariffs",
                                    connector.tariff_ids.join(", ")
                                );

                            if (connector.terms_and_conditions)
                                CreateProperty(
                                    connectorPropertiesDiv,
                                    "terms",
                                    "Terms",
                                    connector.terms_and_conditions
                                );

                            const datesDiv      = connectorDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            datesDiv.className  = "dates properties";

                            if (connector.created)
                                CreateProperty(
                                    datesDiv,
                                    "created",
                                    "Created:",
                                    connector.created
                                )

                            CreateProperty(
                                datesDiv,
                                "lastUpdated",
                                "Last updated:",
                                connector.last_updated
                            )

                        }
                    }

                    const datesDiv      = evseDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    datesDiv.className  = "dates properties";

                    if (evse.created)
                        CreateProperty(
                            datesDiv,
                            "created",
                            "Created:",
                            evse.created
                        )

                    CreateProperty(
                        datesDiv,
                        "lastUpdated",
                        "Last updated:",
                        evse.last_updated
                    )

                }
            }

            const datesDiv      = locationAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            datesDiv.className  = "dates properties";

            if (location.created)
                CreateProperty(
                    datesDiv,
                    "created",
                    "Created:",
                    location.created
                )

            CreateProperty(
                datesDiv,
                "lastUpdated",
                "Last updated:",
                location.last_updated
            )

        },

        // table view
        (tariffs, tariffsDiv) => {
        },

        // linkPrefix
        null,//tariff => "",
        SearchResultsMode.listView,

        context => {
            //statusFilterSelect.onchange = () => {
            //    context.Search(true);
            //}
        }

    );

}
