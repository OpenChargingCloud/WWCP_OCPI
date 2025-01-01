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

function StartSessions()
{

    const common                       = GetDefaults();
    common.topLeft.innerHTML           = "/Sessions"
    common.menuVersions.classList.add("activated");
    common.menuVersions.href           = "../../versions";

    OCPIStartSearch2<ISessionMetadata, ISession>(

        window.location.href,
        () => {
            return "";
        },
        metadata => { },
        "session",
        session => session.id,
        "sessions",
        "sessions",

        // list view
        (resultCounter,
         session,
         sessionAnchor) => {

            const locationCounterDiv      = sessionAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            locationCounterDiv.className  = "counter";
            locationCounterDiv.innerHTML  = resultCounter.toString() + ".";

            const sessionIdDiv            = sessionAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            sessionIdDiv.className        = "id";
            sessionIdDiv.innerHTML        = session.id;

            const propertiesDiv           = sessionAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            propertiesDiv.className       = "properties";

            CreateProperty(
                propertiesDiv,
                "start",
                "Start",
                session.start_datetime
            )

            if (session.end_datetime) {
                CreateProperty(
                    propertiesDiv,
                    "end",
                    "end",
                    session.end_datetime
                )
            }

            CreateProperty(
                propertiesDiv,
                "kWh",
                "kWh",
                session.kwh.toString()
            )

            CreateProperty(
                propertiesDiv,
                "authId",
                "Auth Id",
                session.auth_id
            )

            CreateProperty(
                propertiesDiv,
                "authMethod",
                "Auth Method",
                session.auth_method
            )

            CreateProperty(
                propertiesDiv,
                "location",
                "Location",
                "//ToDo: session.location"
            )

            if (session.meter_id) {
                CreateProperty(
                    propertiesDiv,
                    "meterId",
                    "Meter Id",
                    session.meter_id
                )
            }

            CreateProperty(
                propertiesDiv,
                "currency",
                "Currency",
                session.currency
            )


            const chargingPeriodsDiv      = propertiesDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            chargingPeriodsDiv.className  = "chargingPeriods";

            let numberOfChargingPeriod = 1;

            if (session.charging_periods) {
                for (const chargingPeriod of session.charging_periods) {

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

            const datesDiv      = propertiesDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            datesDiv.className  = "dates properties";

            if (session.created)
                CreateProperty(
                    datesDiv,
                    "created",
                    "Created:",
                    session.created
                )

            CreateProperty(
                datesDiv,
                "lastUpdated",
                "Last updated:",
                session.last_updated
            )

        },

        // table view
        (sessions, sessionsDiv) => {
        },

        // linkPrefix
        null,//session => "",
        SearchResultsMode.listView,

        context => {
            //statusFilterSelect.onchange = () => {
            //    context.Search(true);
            //}
        }

    );

}
