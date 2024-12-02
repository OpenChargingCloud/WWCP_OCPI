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

function StartLocationStatistics()
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


    const common                   = GetDefaults();
    common.topLeft.innerHTML       = "/Location"
    common.menuVersions.classList.add("activated");
    common.menuVersions.href       = "../../versions";

    const locationInfosDiv         = document.getElementById("locationInfos")                     as HTMLDivElement;
    const numberOfLocationsDiv     = locationInfosDiv.querySelector("#numberOfLocations")         as HTMLDivElement;
    const numberOfEVSEsDiv         = locationInfosDiv.querySelector("#numberOfEVSEs")             as HTMLDivElement;

    let   totalNumberOfLocations   = 0;
    let   totalNumberOfEVSEs       = 0;

    const evseStatusStatisticsDiv  = locationInfosDiv.querySelector("#evseStatusStatistics")      as HTMLDivElement;
    const evseStatusMap            = new Map<string, number>();

    const tariffStatisticsDiv      = locationInfosDiv.querySelector("#tariffStatistics")          as HTMLDivElement;
    const tariffsMap               = new Map<string, number>();


    OCPIGetCollection<ILocationMetadata, ILocation>(

        window.location.href.replace(/\/locationStatistics$/, "/locations"),
        metadata => {

            //if (metadata["description"] != null && firstValue(metadata["description"]) != null)
            //{
            //    (communicatorDescription.querySelector("#language") as HTMLDivElement).innerText = firstKey  (metadata["description"]);
            //    (communicatorDescription.querySelector("#I18NText") as HTMLDivElement).innerText = firstValue(metadata["description"]);
            //}

        },
        "locations",

        (resultCounter,
         location) => {

            totalNumberOfLocations++;

            if (location.evses)
                totalNumberOfEVSEs += location.evses.length;

            if (location.evses) {
                for (var evse of location.evses) {

                    if (evse.connectors) {
                        for (var connector of evse.connectors) {

                            if (connector.tariff_id) {

                                // Tariffs map
                                if (!tariffsMap.has(connector.tariff_id)) {
                                    tariffsMap.set(connector.tariff_id, 0);
                                }

                                tariffsMap.set(
                                    connector.tariff_id,
                                    tariffsMap.get(connector.tariff_id) + 1
                                );

                            }

                        }
                    }

                    // EVSE Status map
                    if (!evseStatusMap.has(evse.status)) {
                        evseStatusMap.set(evse.status, 0);
                    }

                    evseStatusMap.set(
                        evse.status,
                        evseStatusMap.get(evse.status) + 1
                    );

                }
            }

            numberOfLocationsDiv.innerHTML  = totalNumberOfLocations.toString();
            numberOfEVSEsDiv.innerHTML      = totalNumberOfEVSEs.toString();

        },

        () => {

            for (const [evseStatus, count] of evseStatusMap) {
                CreateLine(
                    evseStatusStatisticsDiv,
                    "evseStatus",
                    evseStatus,
                    count.toString()
                )
            }

            for (const [tariffId, count] of tariffsMap) {
                CreateLine(
                    tariffStatisticsDiv,
                    "tariff",
                    tariffId,
                    count.toString()
                )
            }

        }

    );


}
