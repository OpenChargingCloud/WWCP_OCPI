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

function StartCDRs()
{

    const common                       = GetDefaults();
    common.topLeft.innerHTML           = "/CDRs"
    common.menuVersions.classList.add("activated");
    common.menuVersions.href           = "../../versions";

    OCPIStartSearch2<ICDRMetadata, ICDR>(

        window.location.href,
        () => {
            return "";
        },
        metadata => { },
        "cdr",
        cdr => cdr.id,
        "cdrs",
        "cdrs",

        // list view
        (resultCounter,
         cdr,
         cdrAnchor) => {

            const locationCounterDiv      = cdrAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            locationCounterDiv.className  = "counter";
            locationCounterDiv.innerHTML  = resultCounter.toString() + ".";

            const cdrIdDiv                = cdrAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            cdrIdDiv.className            = "id";
            cdrIdDiv.innerHTML            = cdr.id;

            const propertiesDiv           = cdrAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            propertiesDiv.className       = "properties";

            CreateProperty(
                propertiesDiv,
                "start",
                "Start",
                cdr.start_date_time
            )

            CreateProperty(
                propertiesDiv,
                "end",
                "end",
                cdr.end_date_time
            )

            if (cdr.authorization_reference)
                CreateProperty(
                    propertiesDiv,
                    "authorizationReference",
                    "Auth Ref",
                    cdr.authorization_reference
                )

            CreateProperty(
                propertiesDiv,
                "authMethod",
                "Auth Method",
                cdr.auth_method
            )

            CreateProperty(
                propertiesDiv,
                "location",
                "Location",
                "//ToDo: cdr.location"
            )

            if (cdr.meter_id) {
                CreateProperty(
                    propertiesDiv,
                    "meterId",
                    "Meter Id",
                    cdr.meter_id
                )
            }

            CreateProperty(
                propertiesDiv,
                "currency",
                "Currency",
                cdr.currency
            )

            const tariffsDiv      = propertiesDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            tariffsDiv.className  = "tariffs";

            let numberOfTariff = 1;

            if (cdr.tariffs) {
                for (const tariff of cdr.tariffs) {

                    const tariffDiv          = tariffsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    tariffDiv.className      = "tariff";

                    const numberDiv          = tariffDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    numberDiv.className      = "number";
                    numberDiv.innerHTML      = (numberOfTariff++) + ".";

                    const tariffIdDiv        = tariffDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    tariffIdDiv.className    = "tariffId";
                    tariffIdDiv.innerHTML    = tariff.id;

                    // ...

                }
            }

            const chargingPeriodsDiv      = propertiesDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            chargingPeriodsDiv.className  = "chargingPeriods";

            let numberOfChargingPeriod = 1;

            if (cdr.charging_periods) {
                for (const chargingPeriod of cdr.charging_periods) {

                    const chargingPeriodDiv      = chargingPeriodsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    chargingPeriodDiv.className  = "chargingPeriod";

                    const numberDiv              = chargingPeriodDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    numberDiv.className          = "number";
                    numberDiv.innerHTML          = (numberOfChargingPeriod++) + ".";

                    const startDiv               = chargingPeriodDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    startDiv.className           = "start";
                    startDiv.innerHTML           = chargingPeriod.start_date_time;

                    const dimensionsDiv          = chargingPeriodDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    dimensionsDiv.className      = "dimensions";

                    for (const dimension of chargingPeriod.dimensions) {

                        const dimensionTypeDiv        = dimensionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        dimensionTypeDiv.className    = "type";
                        dimensionTypeDiv.innerHTML    = dimension.type;

                        const dimensionVolumeDiv      = dimensionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        dimensionVolumeDiv.className  = "volume";
                        dimensionVolumeDiv.innerHTML  = dimension.volume.toString();

                    }

                }
            }

            CreateProperty(
                propertiesDiv,
                "totalCost",
                "Total Cost",
                cdr.total_cost.toString()
            )

            CreateProperty(
                propertiesDiv,
                "totalEnergy",
                "Total Energy",
                cdr.total_energy.toString()
            )

            CreateProperty(
                propertiesDiv,
                "totalTime",
                "Total Time",
                cdr.total_time.toString()
            )

            if (cdr.total_parking_time)
                CreateProperty(
                    propertiesDiv,
                    "totalParkingTime",
                    "Total Parking Time",
                    cdr.total_parking_time.toString()
                )

            if (cdr.remark)
                CreateProperty(
                    propertiesDiv,
                    "remark",
                    "Remark",
                    cdr.remark
                )

            const datesDiv      = cdrAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            datesDiv.className  = "dates properties";

            if (cdr.created)
                CreateProperty(
                    datesDiv,
                    "created",
                    "Created:",
                    cdr.created
                )

            CreateProperty(
                datesDiv,
                "lastUpdated",
                "Last updated:",
                cdr.last_updated
            )

        },

        // table view
        (cdrs, cdrsDiv) => {
        },

        // linkPrefix
        null,//cdr => "",
        SearchResultsMode.listView,

        context => {
            //statusFilterSelect.onchange = () => {
            //    context.Search(true);
            //}
        }

    );

}
