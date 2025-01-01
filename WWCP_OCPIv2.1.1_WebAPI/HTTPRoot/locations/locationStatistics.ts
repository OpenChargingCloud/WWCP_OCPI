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

function StartLocationStatistics()
{

    function CreateLine(parent:       HTMLDivElement | HTMLAnchorElement,
                        className:    string,
                        key:          string,
                        innerHTML1:   string | HTMLDivElement,
                        innerHTML2?:  string | HTMLDivElement): HTMLDivElement {

        const rowDiv = parent.appendChild(document.createElement('div')) as HTMLDivElement;
        rowDiv.className = "row";

        // key
        const keyDiv = rowDiv.appendChild(document.createElement('div')) as HTMLDivElement;
        keyDiv.className = "key";
        keyDiv.innerHTML = key;


        // value 1
        const valueDiv = rowDiv.appendChild(document.createElement('div')) as HTMLDivElement;
        valueDiv.className = "value " + className;

        if (typeof innerHTML1 === 'string')
            valueDiv.innerHTML = innerHTML1;

        else if (innerHTML1 instanceof HTMLDivElement)
            valueDiv.appendChild(innerHTML1);


        // value 2
        if (innerHTML2) {

            const value2Div = rowDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            value2Div.className = "value " + className;

            if (typeof innerHTML2 === 'string')
                value2Div.innerHTML = innerHTML2;

            else if (innerHTML2 instanceof HTMLDivElement)
                value2Div.appendChild(innerHTML2);

        }

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
    const tariffsMapActive         = new Map<string, number>();
    const tariffsMapRemoved        = new Map<string, number>();
    let   noTariffCounter          = 0;
    let   noTariffREMOVEDCounter   = 0;


    OCPIGetCollection<ILocationMetadata, ILocation>(

        window.location.href.replace(/\/locationStatistics$/, "/locations"),
        metadata => { },
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

                                // Tariffs for EVSEs in use...
                                if (!tariffsMapActive.has(connector.tariff_id))
                                    tariffsMapActive.set(connector.tariff_id, 0);

                                // ...or for removed EVSEs
                                if (!tariffsMapRemoved.has(connector.tariff_id))
                                    tariffsMapRemoved.set(connector.tariff_id, 0);


                                if (evse.status === "REMOVED")
                                    tariffsMapRemoved.set(
                                        connector.tariff_id,
                                        tariffsMapRemoved.get(connector.tariff_id) + 1
                                    );

                                else
                                    tariffsMapActive.set(
                                        connector.tariff_id,
                                        tariffsMapActive.get(connector.tariff_id) + 1
                                    );

                            }
                            else {
                                if (evse.status === "REMOVED")
                                    noTariffREMOVEDCounter++;
                                else
                                    noTariffCounter++;
                            }

                        }
                    }

                    // EVSE Status map
                    if (!evseStatusMap.has(evse.status))
                        evseStatusMap.set(evse.status, 0);

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


            CreateLine(
                tariffStatisticsDiv,
                "tariff",
                "&lt;no tariff&gt;",
                noTariffCounter.toString(),
                "(" + noTariffREMOVEDCounter.toString() + ")"
            )

            for (const [tariffId, count] of tariffsMapActive) {
                CreateLine(
                    tariffStatisticsDiv,
                    "tariff",
                    tariffId,
                    count.toString(),
                    "(" + tariffsMapRemoved.get(tariffId).toString() + ")"
                )
            }

        }

    );

}
