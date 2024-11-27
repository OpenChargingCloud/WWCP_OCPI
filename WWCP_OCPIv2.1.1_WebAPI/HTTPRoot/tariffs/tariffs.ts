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


    const locationInfosDiv             = document.getElementById("locationInfos")                     as HTMLDivElement;
    const locationsDiv                 = locationInfosDiv.querySelector("#locations")                 as HTMLDivElement;

    const numberOfTariffsDiv           = locationInfosDiv.querySelector("#numberOfTariffs")           as HTMLDivElement;
 
    let   totalNumberOfEVSEs           = 0;


    OCPIGet(window.location.href,

            (status, response) => {

                try
                {

                    const ocpiResponse = JSON.parse(response) as IOCPIResponse;

                    if (ocpiResponse?.data != undefined  &&
                        ocpiResponse?.data != null       &&
                        Array.isArray(ocpiResponse.data) &&
                        ocpiResponse.data.length > 0)
                    {

                        numberOfTariffsDiv.innerHTML = ocpiResponse.data.length.toString();

                        let numberOfLocation = 1;

                        for (const tariff of (ocpiResponse.data as ITariff[])) {

                            totalNumberOfEVSEs++;

                            const tariffDiv = locationsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            tariffDiv.className = "tariff";

                            const tariffIdDiv = tariffDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            tariffIdDiv.className = "id";
                            tariffIdDiv.innerHTML = tariff.id;

                            const tariffAltTextsDiv = tariffDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            tariffAltTextsDiv.className = "altTexts";

                            for (const tariff_alt_text_instance of tariff.tariff_alt_text) {

                                const altTextDiv = tariffAltTextsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                altTextDiv.className = "altText";
                                altTextDiv.innerHTML = "(" + tariff_alt_text_instance.language + ") " + tariff_alt_text_instance.text;
                                // tariff_alt_url

                            }

                            // energy_mix


                            const tariffElementsDiv = tariffDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            tariffAltTextsDiv.className = "tariffElements";

                            for (const tariffElement of tariff.elements) {

                                const tariffElementDiv = tariffElementsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                tariffElementDiv.className = "tariffElement";

                                const priceComponentsDiv = tariffElementDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                priceComponentsDiv.className = "priceComponents";

                                for (const priceComponent of tariffElement.price_components) {

                                    const priceComponentDiv = priceComponentsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                    priceComponentDiv.className = "priceComponent";


                                    const priceComponentTypeDiv = priceComponentsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                    priceComponentTypeDiv.className = "priceComponentType";
                                    priceComponentTypeDiv.innerHTML = priceComponent.type.toString();

                                    const priceComponentPriceDiv = priceComponentsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                    priceComponentPriceDiv.className = "priceComponentPrice";
                                    priceComponentPriceDiv.innerHTML = priceComponent.price.toString();

                                    const priceComponentStepSizeDiv = priceComponentsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                    priceComponentStepSizeDiv.className = "priceComponentStepSize";
                                    priceComponentStepSizeDiv.innerHTML = priceComponent.step_size.toString();

                                }


                            }

                        }

                        //    const propertiesDiv         = tariffDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        //    propertiesDiv.className     = "properties";

                        //    CreateLine(
                        //        propertiesDiv,
                        //        "details",
                        //        "Type",
                        //        location.type
                        //    )

                        //    // address
                        //    // city
                        //    // postal_code
                        //    // country
                        //    // coordinates
                        //    // related_locations
                        //    // directions
                        //    // facilities
                        //    // time_zone
                        //    // opening_times
                        //    // charging_when_closed
                        //    // images
                        //    // energy_mix
                        //    // publish (PlugSurfing extension)

                        //    if (location.operator) {
                        //        CreateLine(
                        //            propertiesDiv,
                        //            "businessDetails",
                        //            "Operator",
                        //           (location.operator.website
                        //                ? "<a href=\"" + location.operator.website + "\">" + location.operator.name + "</a>"
                        //                : location.operator.name)
                        //        )
                        //    }

                        //    if (location.suboperator) {
                        //        CreateLine(
                        //            propertiesDiv,
                        //            "businessDetails",
                        //            "Suboperator",
                        //           (location.suboperator.website
                        //                ? "<a href=\"" + location.suboperator.website + "\">" + location.suboperator.name + "</a>"
                        //                : location.suboperator.name)
                        //        )
                        //    }

                        //    if (location.owner) {
                        //        CreateLine(
                        //            propertiesDiv,
                        //            "businessDetails",
                        //            "Owner",
                        //            (location.owner.website
                        //                ? "<a href=\"" + location.owner.website + "\">" + location.owner.name + "</a>"
                        //                : location.owner.name)
                        //        )
                        //    }

                        //    const evsesDiv        = tariffDiv.appendChild(document.createElement('a')) as HTMLAnchorElement;
                        //    evsesDiv.className    = "evses";

                        //    let   numberOfEVSE    = 1;
                        //    const numberOfEVSEs   = location.evses?.length ?? 0;

                        //    if (location.evses) {
                        //        for (var evse of location.evses) {

                        //            const evseDiv           = evsesDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        //            evseDiv.className       = "evse evseStatus_" + evse.status;

                        //            const statusDiv         = evseDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        //            statusDiv.className     = "evseStatus evseStatus_" + evse.status;
                        //            statusDiv.innerHTML     = evse.status;

                        //            const evesIdDiv         = evseDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        //            evesIdDiv.className     = "evseId";
                        //            evesIdDiv.innerHTML     = (numberOfEVSE++) + ". " + evse.uid + (evse.evse_id && evse.evse_id != evse.uid ? " (" + evse.evse_id + ")" : "");
                        //                                      // physical_reference

                        //            // capabilities
                        //            // energy_meter (OCPI Calibration Law Extentions)
                        //            // floor_level
                        //            // coordinates
                        //            // directions
                        //            // parking_restrictions
                        //            // images
                        //            // last_updated

                        //            const connectorsDiv     = evseDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        //            connectorsDiv.className = "connectors";

                        //            if (evse.connectors) {
                        //                for (var connector of evse.connectors) {

                        //                    const connectorDiv     = connectorsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        //                    connectorDiv.className = "connector";

                        //                    const connectorInfoDiv = connectorDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        //                    connectorInfoDiv.className = "connectorInfo";
                        //                    connectorInfoDiv.innerHTML = connector.id + ". " + connector.standard + ", " + connector.format + ", " + connector.amperage + " A, " + connector.voltage + " V, " + connector.power_type;

                        //                    if (connector.tariff_id) {
                        //                        const tariffInfoDiv = connectorDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        //                        tariffInfoDiv.className = "tariffInfo";
                        //                        tariffInfoDiv.innerHTML = "Tariff: " + connector.tariff_id + (connector.terms_and_conditions
                        //                                                                                          ? ", terms: " + connector.terms_and_conditions
                        //                                                                                          : "");
                        //                    }

                        //                    // last_updated

                        //                }
                        //            }

                        //            switch (evse.status) {

                        //                case "AVAILABLE":
                        //                    evsesAvailable++;
                        //                    break;

                        //                case "CHARGING":
                        //                    evsesCharging++;
                        //                    break;

                        //                case "OUTOFORDER":
                        //                    evsesOutOfOrder++;
                        //                    break;

                        //                case "INOPERATIVE":
                        //                    evsesInoperative++;
                        //                    break;

                        //                case "UNKNOWN":
                        //                    evsesUnkown++;
                        //                    break;

                        //                case "REMOVED":
                        //                    evsesRemoved++;
                        //                    break;

                        //                default:
                        //                    evsesX++;
                        //                    break;

                        //            }

                        //        }
                        //    }

                        //}

                        //numberOfEVSEsDiv.innerHTML            = totalNumberOfEVSEs.toString();


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
