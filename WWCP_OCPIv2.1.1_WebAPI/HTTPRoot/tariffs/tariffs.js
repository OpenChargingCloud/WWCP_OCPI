///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
function StartTariffs() {
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
    common.topLeft.innerHTML = "/Tariffs";
    common.menuVersions.classList.add("activated");
    common.menuVersions.href = "../../versions";
    const tariffInfosDiv = document.getElementById("tariffInfos");
    const tariffsDiv = tariffInfosDiv.querySelector("#tariffs");
    const numberOfTariffsDiv = tariffInfosDiv.querySelector("#numberOfTariffs");
    let totalNumberOfEVSEs = 0;
    OCPIGet(window.location.href, (status, response) => {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t, _u, _v, _w, _x;
        try {
            const ocpiResponse = JSON.parse(response);
            if ((ocpiResponse === null || ocpiResponse === void 0 ? void 0 : ocpiResponse.data) != undefined &&
                (ocpiResponse === null || ocpiResponse === void 0 ? void 0 : ocpiResponse.data) != null &&
                Array.isArray(ocpiResponse.data) &&
                ocpiResponse.data.length > 0) {
                numberOfTariffsDiv.innerHTML = ocpiResponse.data.length.toString();
                for (const tariff of ocpiResponse.data) {
                    totalNumberOfEVSEs++;
                    const tariffDiv = tariffsDiv.appendChild(document.createElement('div'));
                    tariffDiv.className = "tariff";
                    const tariffIdDiv = tariffDiv.appendChild(document.createElement('div'));
                    tariffIdDiv.className = "id";
                    tariffIdDiv.innerHTML = tariff.id;
                    if (tariff.tariff_alt_url) {
                        const altURLDiv = tariffDiv.appendChild(document.createElement('div'));
                        altURLDiv.className = "altURL";
                        altURLDiv.innerHTML = "<a href=\"" + tariff.tariff_alt_url + "\">" + tariff.tariff_alt_url + "</a>";
                    }
                    const tariffAltTextsDiv = tariffDiv.appendChild(document.createElement('div'));
                    tariffAltTextsDiv.className = "altTexts";
                    for (const tariff_alt_text_instance of tariff.tariff_alt_text) {
                        const altTextDiv = tariffAltTextsDiv.appendChild(document.createElement('div'));
                        altTextDiv.className = "altText";
                        altTextDiv.innerHTML = "(" + tariff_alt_text_instance.language + ") " + tariff_alt_text_instance.text;
                    }
                    if (tariff.energy_mix) {
                        const altURLDiv = tariffDiv.appendChild(document.createElement('div'));
                        altURLDiv.className = "altURL";
                        altURLDiv.innerHTML = "<a href=\"" + tariff.tariff_alt_url + "\">" + tariff.tariff_alt_url + "</a>";
                    }
                    const tariffElementsDiv = tariffDiv.appendChild(document.createElement('div'));
                    tariffElementsDiv.className = "tariffElements";
                    for (const tariffElement of tariff.elements) {
                        const tariffElementDiv = tariffElementsDiv.appendChild(document.createElement('div'));
                        tariffElementDiv.className = "tariffElement";
                        const priceComponentsDiv = tariffElementDiv.appendChild(document.createElement('div'));
                        priceComponentsDiv.className = "priceComponents";
                        for (const priceComponent of tariffElement.price_components) {
                            const priceComponentDiv = priceComponentsDiv.appendChild(document.createElement('div'));
                            priceComponentDiv.className = "priceComponent";
                            const priceComponentTypeDiv = priceComponentDiv.appendChild(document.createElement('div'));
                            priceComponentTypeDiv.className = "type";
                            priceComponentTypeDiv.innerHTML = priceComponent.type.toString();
                            const priceComponentPriceDiv = priceComponentDiv.appendChild(document.createElement('div'));
                            priceComponentPriceDiv.className = "price";
                            priceComponentPriceDiv.innerHTML = priceComponent.price.toString() + " " + tariff.currency;
                            if (priceComponent.type !== "FLAT") {
                                const priceComponentStepSizeDiv = priceComponentDiv.appendChild(document.createElement('div'));
                                priceComponentStepSizeDiv.className = "stepSize";
                                priceComponentStepSizeDiv.innerHTML = priceComponent.step_size.toString();
                            }
                        }
                        if (((_a = tariffElement.restrictions) === null || _a === void 0 ? void 0 : _a.start_time) ||
                            ((_b = tariffElement.restrictions) === null || _b === void 0 ? void 0 : _b.end_time) ||
                            ((_c = tariffElement.restrictions) === null || _c === void 0 ? void 0 : _c.start_date) ||
                            ((_d = tariffElement.restrictions) === null || _d === void 0 ? void 0 : _d.end_date) ||
                            ((_e = tariffElement.restrictions) === null || _e === void 0 ? void 0 : _e.min_kwh) ||
                            ((_f = tariffElement.restrictions) === null || _f === void 0 ? void 0 : _f.max_kwh) ||
                            ((_g = tariffElement.restrictions) === null || _g === void 0 ? void 0 : _g.min_power) ||
                            ((_h = tariffElement.restrictions) === null || _h === void 0 ? void 0 : _h.max_power) ||
                            ((_j = tariffElement.restrictions) === null || _j === void 0 ? void 0 : _j.min_duration) ||
                            ((_k = tariffElement.restrictions) === null || _k === void 0 ? void 0 : _k.max_duration) ||
                            ((_l = tariffElement.restrictions) === null || _l === void 0 ? void 0 : _l.day_of_week)) {
                            const tariffRestrictionsDiv = tariffElementDiv.appendChild(document.createElement('div'));
                            tariffRestrictionsDiv.className = "tariffRestrictions";
                            tariffRestrictionsDiv.innerHTML = "Restrictions";
                            const restrictionsDiv = tariffRestrictionsDiv.appendChild(document.createElement('div'));
                            restrictionsDiv.className = "restrictions";
                            if ((_m = tariffElement.restrictions) === null || _m === void 0 ? void 0 : _m.start_time) {
                                const restrictionDiv = restrictionsDiv.appendChild(document.createElement('div'));
                                restrictionDiv.className = "restriction";
                                const restrictionKeyDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionKeyDiv.className = "key";
                                restrictionKeyDiv.innerHTML = "Start Time";
                                const restrictionValueDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionValueDiv.className = "value";
                                restrictionValueDiv.innerHTML = tariffElement.restrictions.start_time;
                            }
                            if ((_o = tariffElement.restrictions) === null || _o === void 0 ? void 0 : _o.end_time) {
                                const restrictionDiv = restrictionsDiv.appendChild(document.createElement('div'));
                                restrictionDiv.className = "restriction";
                                const restrictionKeyDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionKeyDiv.className = "key";
                                restrictionKeyDiv.innerHTML = "End Time";
                                const restrictionValueDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionValueDiv.className = "value";
                                restrictionValueDiv.innerHTML = tariffElement.restrictions.end_time;
                            }
                            if ((_p = tariffElement.restrictions) === null || _p === void 0 ? void 0 : _p.start_date) {
                                const restrictionDiv = restrictionsDiv.appendChild(document.createElement('div'));
                                restrictionDiv.className = "restriction";
                                const restrictionKeyDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionKeyDiv.className = "key";
                                restrictionKeyDiv.innerHTML = "Start Date";
                                const restrictionValueDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionValueDiv.className = "value";
                                restrictionValueDiv.innerHTML = tariffElement.restrictions.start_date;
                            }
                            if ((_q = tariffElement.restrictions) === null || _q === void 0 ? void 0 : _q.end_date) {
                                const restrictionDiv = restrictionsDiv.appendChild(document.createElement('div'));
                                restrictionDiv.className = "restriction";
                                const restrictionKeyDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionKeyDiv.className = "key";
                                restrictionKeyDiv.innerHTML = "End Date";
                                const restrictionValueDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionValueDiv.className = "value";
                                restrictionValueDiv.innerHTML = tariffElement.restrictions.end_date;
                            }
                            if ((_r = tariffElement.restrictions) === null || _r === void 0 ? void 0 : _r.min_kwh) {
                                const restrictionDiv = restrictionsDiv.appendChild(document.createElement('div'));
                                restrictionDiv.className = "restriction";
                                const restrictionKeyDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionKeyDiv.className = "key";
                                restrictionKeyDiv.innerHTML = "min kWh";
                                const restrictionValueDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionValueDiv.className = "value";
                                restrictionValueDiv.innerHTML = tariffElement.restrictions.min_kwh.toString();
                            }
                            if ((_s = tariffElement.restrictions) === null || _s === void 0 ? void 0 : _s.max_kwh) {
                                const restrictionDiv = restrictionsDiv.appendChild(document.createElement('div'));
                                restrictionDiv.className = "restriction";
                                const restrictionKeyDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionKeyDiv.className = "key";
                                restrictionKeyDiv.innerHTML = "max kWh";
                                const restrictionValueDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionValueDiv.className = "value";
                                restrictionValueDiv.innerHTML = tariffElement.restrictions.max_kwh.toString();
                            }
                            if ((_t = tariffElement.restrictions) === null || _t === void 0 ? void 0 : _t.min_power) {
                                const restrictionDiv = restrictionsDiv.appendChild(document.createElement('div'));
                                restrictionDiv.className = "restriction";
                                const restrictionKeyDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionKeyDiv.className = "key";
                                restrictionKeyDiv.innerHTML = "min power";
                                const restrictionValueDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionValueDiv.className = "value";
                                restrictionValueDiv.innerHTML = tariffElement.restrictions.min_power.toString();
                            }
                            if ((_u = tariffElement.restrictions) === null || _u === void 0 ? void 0 : _u.max_power) {
                                const restrictionDiv = restrictionsDiv.appendChild(document.createElement('div'));
                                restrictionDiv.className = "restriction";
                                const restrictionKeyDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionKeyDiv.className = "key";
                                restrictionKeyDiv.innerHTML = "max power";
                                const restrictionValueDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionValueDiv.className = "value";
                                restrictionValueDiv.innerHTML = tariffElement.restrictions.max_power.toString();
                            }
                            if ((_v = tariffElement.restrictions) === null || _v === void 0 ? void 0 : _v.min_duration) {
                                const restrictionDiv = restrictionsDiv.appendChild(document.createElement('div'));
                                restrictionDiv.className = "restriction";
                                const restrictionKeyDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionKeyDiv.className = "key";
                                restrictionKeyDiv.innerHTML = "min duration";
                                const restrictionValueDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionValueDiv.className = "value";
                                restrictionValueDiv.innerHTML = tariffElement.restrictions.min_duration.toString();
                            }
                            if ((_w = tariffElement.restrictions) === null || _w === void 0 ? void 0 : _w.max_duration) {
                                const restrictionDiv = restrictionsDiv.appendChild(document.createElement('div'));
                                restrictionDiv.className = "restriction";
                                const restrictionKeyDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionKeyDiv.className = "key";
                                restrictionKeyDiv.innerHTML = "max duration";
                                const restrictionValueDiv = restrictionDiv.appendChild(document.createElement('div'));
                                restrictionValueDiv.className = "value";
                                restrictionValueDiv.innerHTML = tariffElement.restrictions.max_duration.toString();
                            }
                            if ((_x = tariffElement.restrictions) === null || _x === void 0 ? void 0 : _x.day_of_week) {
                                const restrictionStartTimeDiv = restrictionsDiv.appendChild(document.createElement('div'));
                                restrictionStartTimeDiv.className = "restriction";
                                const restrictionStartTimeKeyDiv = restrictionStartTimeDiv.appendChild(document.createElement('div'));
                                restrictionStartTimeKeyDiv.className = "key";
                                restrictionStartTimeKeyDiv.innerHTML = "day of week";
                                const restrictionStartTimeValueDiv = restrictionStartTimeDiv.appendChild(document.createElement('div'));
                                restrictionStartTimeValueDiv.className = "value";
                                for (const dayOfWeek of tariffElement.restrictions.day_of_week)
                                    restrictionStartTimeValueDiv.innerHTML += " " + dayOfWeek;
                            }
                        }
                    }
                    const tariffLastUpdatedDiv = tariffDiv.appendChild(document.createElement('div'));
                    tariffLastUpdatedDiv.className = "lastUpdated";
                    tariffLastUpdatedDiv.innerHTML = "Last updated: " + tariff.last_updated;
                }
            }
        }
        catch (exception) {
        }
    }, (status, statusText, response) => {
    });
    //var refresh = setTimeout(StartDashboard, 30000);
}
//# sourceMappingURL=tariffs.js.map