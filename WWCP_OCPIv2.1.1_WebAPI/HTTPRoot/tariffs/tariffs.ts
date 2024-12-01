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

function StartTariffs()
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
    common.topLeft.innerHTML           = "/Tariffs"
    common.menuVersions.classList.add("activated");
    common.menuVersions.href           = "../../versions";

    //const tariffInfosDiv               = document.getElementById("tariffInfos")                     as HTMLDivElement;
    //const tariffsDiv                   = tariffInfosDiv.querySelector("#tariffs")                   as HTMLDivElement;

    //const numberOfTariffsDiv           = tariffInfosDiv.querySelector("#numberOfTariffs")           as HTMLDivElement;
 
    //let   totalNumberOfEVSEs           = 0;

    //const controlsDiv         = document.       getElementById("controls")              as HTMLDivElement;
    //const searchDiv           = controlsDiv.    querySelector ("#search")               as HTMLDivElement;

    //const lowerButtonsDiv     = document.       getElementById("lowerButtons")          as HTMLDivElement;
    //const downLoadButton      = lowerButtonsDiv.querySelector ("#downLoadButton")       as HTMLAnchorElement;


    OCPIStartSearch2<ITariffMetadata, ITariff>(

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
        "tariff",
        tariff => tariff.id,
        "tariffs",
        "tariffs",

        // list view
        (resultCounter,
         tariff,
         tariffAnchor) => {

            //numberOfTariffsDiv.innerHTML = ocpiResponse.data.length.toString();

            const tariffCounterDiv          = tariffAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            tariffCounterDiv.className      = "counter";
            tariffCounterDiv.innerHTML      = resultCounter.toString() + ".";

            const tariffIdDiv               = tariffAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            tariffIdDiv.className           = "id";
            tariffIdDiv.innerHTML           = tariff.id;

            if (tariff.tariff_alt_url) {
                const altURLDiv             = tariffAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
                altURLDiv.className         = "altURL";
                altURLDiv.innerHTML         = "<a href=\"" + tariff.tariff_alt_url + "\">" + tariff.tariff_alt_url + "</a>";
            }

            const tariffAltTextsDiv         = tariffAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            tariffAltTextsDiv.className     = "altTexts";

            for (const tariff_alt_text_instance of tariff.tariff_alt_text) {

                const altTextDiv                = tariffAltTextsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                altTextDiv.className            = "altText";
                altTextDiv.innerHTML            = "(" + tariff_alt_text_instance.language + ") " + tariff_alt_text_instance.text;

            }

            if (tariff.energy_mix) {
                const altURLDiv             = tariffAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
                altURLDiv.className         = "altURL";
                altURLDiv.innerHTML         = "<a href=\"" + tariff.tariff_alt_url + "\">" + tariff.tariff_alt_url + "</a>";
            }

            const tariffElementsDiv         = tariffAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            tariffElementsDiv.className     = "tariffElements";


            for (const tariffElement of tariff.elements) {

                const tariffElementDiv               = tariffElementsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                tariffElementDiv.className           = "tariffElement";

                const priceComponentsDiv             = tariffElementDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                priceComponentsDiv.className         = "priceComponents";

                for (const priceComponent of tariffElement.price_components) {

                    const priceComponentDiv              = priceComponentsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    priceComponentDiv.className          = "priceComponent";


                    const priceComponentTypeDiv          = priceComponentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    priceComponentTypeDiv.className      = "type";
                    priceComponentTypeDiv.innerHTML      = priceComponent.type.toString();

                    const priceComponentPriceDiv         = priceComponentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    priceComponentPriceDiv.className     = "price";
                    priceComponentPriceDiv.innerHTML     = priceComponent.price.toString() + " " + tariff.currency;

                    if (priceComponent.type !== "FLAT") {
                        const priceComponentStepSizeDiv = priceComponentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        priceComponentStepSizeDiv.className = "stepSize";
                        priceComponentStepSizeDiv.innerHTML = priceComponent.step_size.toString();
                    }

                }


                if (tariffElement.restrictions?.start_time   ||
                    tariffElement.restrictions?.end_time     ||
                    tariffElement.restrictions?.start_date   ||
                    tariffElement.restrictions?.end_date     ||
                    tariffElement.restrictions?.min_kwh      ||
                    tariffElement.restrictions?.max_kwh      ||
                    tariffElement.restrictions?.min_power    ||
                    tariffElement.restrictions?.max_power    ||
                    tariffElement.restrictions?.min_duration ||
                    tariffElement.restrictions?.max_duration ||
                    tariffElement.restrictions?.day_of_week) {

                    const tariffRestrictionsDiv = tariffElementDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    tariffRestrictionsDiv.className = "tariffRestrictions";
                    tariffRestrictionsDiv.innerHTML = "Restrictions";

                    const restrictionsDiv = tariffRestrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    restrictionsDiv.className = "restrictions";


                    if (tariffElement.restrictions?.start_time) {

                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionDiv.className       = "restriction";

                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionKeyDiv.className    = "key";
                        restrictionKeyDiv.innerHTML    = "Start Time";

                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionValueDiv.className  = "value";
                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.start_time;

                    }

                    if (tariffElement.restrictions?.end_time) {

                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionDiv.className       = "restriction";

                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionKeyDiv.className    = "key";
                        restrictionKeyDiv.innerHTML    = "End Time";

                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionValueDiv.className  = "value";
                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.end_time;

                    }

                    if (tariffElement.restrictions?.start_date) {

                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionDiv.className       = "restriction";

                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionKeyDiv.className    = "key";
                        restrictionKeyDiv.innerHTML    = "Start Date";

                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionValueDiv.className  = "value";
                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.start_date;

                    }

                    if (tariffElement.restrictions?.end_date) {

                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionDiv.className       = "restriction";

                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionKeyDiv.className    = "key";
                        restrictionKeyDiv.innerHTML    = "End Date";

                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionValueDiv.className  = "value";
                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.end_date;

                    }

                    if (tariffElement.restrictions?.min_kwh) {

                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionDiv.className       = "restriction";

                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionKeyDiv.className    = "key";
                        restrictionKeyDiv.innerHTML    = "min kWh";

                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionValueDiv.className  = "value";
                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.min_kwh.toString();

                    }

                    if (tariffElement.restrictions?.max_kwh) {

                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionDiv.className       = "restriction";

                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionKeyDiv.className    = "key";
                        restrictionKeyDiv.innerHTML    = "max kWh";

                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionValueDiv.className  = "value";
                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.max_kwh.toString();

                    }

                    if (tariffElement.restrictions?.min_power) {

                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionDiv.className       = "restriction";

                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionKeyDiv.className    = "key";
                        restrictionKeyDiv.innerHTML    = "min power";

                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionValueDiv.className  = "value";
                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.min_power.toString();

                    }

                    if (tariffElement.restrictions?.max_power) {

                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionDiv.className       = "restriction";

                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionKeyDiv.className    = "key";
                        restrictionKeyDiv.innerHTML    = "max power";

                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionValueDiv.className  = "value";
                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.max_power.toString();

                    }

                    if (tariffElement.restrictions?.min_duration) {

                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionDiv.className       = "restriction";

                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionKeyDiv.className    = "key";
                        restrictionKeyDiv.innerHTML    = "min duration";

                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionValueDiv.className  = "value";
                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.min_duration.toString();

                    }

                    if (tariffElement.restrictions?.max_duration) {

                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionDiv.className       = "restriction";

                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionKeyDiv.className    = "key";
                        restrictionKeyDiv.innerHTML    = "max duration";

                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionValueDiv.className  = "value";
                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.max_duration.toString();

                    }

                    if (tariffElement.restrictions?.day_of_week) {

                        const restrictionStartTimeDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionStartTimeDiv.className       = "restriction";

                        const restrictionStartTimeKeyDiv        = restrictionStartTimeDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionStartTimeKeyDiv.className    = "key";
                        restrictionStartTimeKeyDiv.innerHTML    = "day of week";

                        const restrictionStartTimeValueDiv      = restrictionStartTimeDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        restrictionStartTimeValueDiv.className  = "value";

                        for (const dayOfWeek of tariffElement.restrictions.day_of_week)
                            restrictionStartTimeValueDiv.innerHTML  += " " + dayOfWeek;

                    }

                }

            }


            const tariffLastUpdatedDiv      = tariffAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            tariffLastUpdatedDiv.className  = "lastUpdated";
            tariffLastUpdatedDiv.innerHTML  = "Last updated: " + tariff.last_updated;

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
