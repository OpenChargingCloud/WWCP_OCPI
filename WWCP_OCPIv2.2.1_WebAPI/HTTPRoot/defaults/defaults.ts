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


let topLeft: HTMLDivElement = null;

interface ICommons {
    topLeft:                        HTMLDivElement;
    menuVersions:                   HTMLAnchorElement;
    menuRemoteParties:              HTMLAnchorElement;
}

interface TMetadataDefaults {
    totalCount:    number;
    filteredCount: number;
}

interface ISearchResult<T> {
    totalCount:                      number;
    filteredCount:                   number;
    searchResults:                   Array<T>;
}

interface SearchFilter {
    (): string;
}

interface SearchStartUp<TMetadata> {
    (json: TMetadata): void;
}

interface SearchListView<TSearchResult> {
    (resultCounter:    number,
     searchResult:     TSearchResult,
     searchResultDiv:  HTMLAnchorElement): void;
}

interface SearchTableView<TSearchResult> {
    (searchResult:     Array<TSearchResult>,
     searchResultDiv:  HTMLDivElement): void;
}

interface StatisticsDelegate<TSearchResult> {
    (resultCounter:    number,
     searchResult:     TSearchResult): void;
}
interface StatisticsFinishedDelegate<TSearchResult> {
    (resultCounter:    number): void;
}



interface SearchResult2Link<TSearchResult> {
    (searchResult: TSearchResult): string;
}

interface SearchContext {
    (context: any): void;
}

enum SearchResultsMode {
    listView,
    tableView
}



interface IOCPIResponse {
    data:                            any;
    status_code:                     number;
    status_message?:                 string;
    timestamp:                       Date;
}


//#region EncodeToken(AccessToken)

function EncodeToken(AccessToken: string) {

    const buffer = [];

    for (let i = AccessToken.length - 1; i >= 0; i--) {
        buffer.unshift(['&#', AccessToken.charCodeAt(i), ';'].join(''));
    }

    return buffer.join('');

}

//#endregion

// #region OCPIGet(RessourceURI, OnSuccess, OnError)

function OCPIGet(RessourceURI: string,
                 OnSuccess: (httpStatusCode: number, httpContent: string, httpHeaders: (key: string) => string | null) => void,
                 OnError:   (httpStatusCode: number, httpContent: string, httpHeaders: (key: string) => string | null) => void) {

    const ajax = new XMLHttpRequest();
    ajax.open("GET", RessourceURI, true);
    ajax.setRequestHeader("Accept",   "application/json; charset=UTF-8");
    ajax.setRequestHeader("X-Portal", "true");

    const accessToken          = localStorage.getItem("ocpiAccessToken");
    const accessTokenEncoding  = localStorage.getItem("ocpiAccessTokenEncoding");

    if (accessToken)
        ajax.setRequestHeader("Authorization", "Token " + (accessTokenEncoding === "base64" ? btoa(accessToken) : accessToken));

    ajax.onreadystatechange = function () {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            if (this.status >= 100 && this.status < 300)
                OnSuccess?.(
                    this.status,
                    ajax.responseText,
                    (key: string) => ajax.getResponseHeader(key)
                );

            else
                OnError?.(
                    this.status,
                    ajax.responseText,
                    (key: string) => ajax.getResponseHeader(key)
                );

        }

    }

    ajax.send();

}

// #endregion


async function OCPIGetAsync(RessourceURI: string): Promise<[IOCPIResponse, (key: string) => string | null]> {

    return new Promise((resolve, reject) => {

        const ajax = new XMLHttpRequest();
        ajax.open("GET", RessourceURI, true);
        ajax.setRequestHeader("Accept",   "application/json; charset=UTF-8");
        ajax.setRequestHeader("X-Portal", "true");

        const accessToken         = localStorage.getItem("ocpiAccessToken");
        const accessTokenEncoding = localStorage.getItem("ocpiAccessTokenEncoding");

        if (accessToken)
            ajax.setRequestHeader("Authorization", "Token " + (accessTokenEncoding === "base64" ? btoa(accessToken) : accessToken));

        ajax.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status >= 100 && this.status < 300) {
                    try {

                        const ocpiResponse = JSON.parse(ajax.responseText) as IOCPIResponse;

                        if (ocpiResponse.status_code >= 1000 &&
                            ocpiResponse.status_code <  2000) {
                            resolve([ocpiResponse, (key: string) => ajax.getResponseHeader(key)]);
                        }
                        else
                            reject(new Error(ocpiResponse.status_code + (ocpiResponse.status_message ? ": " + ocpiResponse.status_message : "")));

                    }
                    catch (exception) {
                        reject(new Error(exception));
                    }
                } else {
                    reject(new Error(`HTTP Status Code ${this.status}: ${ajax.responseText}`));
                }
            }
        };

        ajax.send();

    });

}


function OCPIStartSearch<TSearchResult>(requestURL:   string,
                                        nameOfItem:   string,
                                        idOfItem:     (searchResult: TSearchResult) => string,
                                        nameOfItems:  string,
                                        nameOfItems2: string,
                                        doListView:   SearchListView<TSearchResult>,
                                        doTableView:  SearchTableView<TSearchResult>,
                                        linkPrefix?:  SearchResult2Link<TSearchResult>,
                                        startView?:   SearchResultsMode,
                                        context?:     SearchContext) {

    return OCPIStartSearch2<any, TSearchResult>(
               requestURL,
               () => "",
               () => { },
               nameOfItem,
               idOfItem,
               nameOfItems,
               nameOfItems2,
               doListView,
               doTableView,
               linkPrefix,
               startView,
               context
           );

}

function OCPIStartSearch2<TMetadata extends TMetadataDefaults, TSearchResult>(requestURL:     string,
                                                                              searchFilters:  SearchFilter,
                                                                              doStartUp:      SearchStartUp<TMetadata>,
                                                                              nameOfItem:     string,
                                                                              idOfItem:       (searchResult: TSearchResult) => string,
                                                                              nameOfItems:    string,
                                                                              nameOfItems2:   string,
                                                                              doListView:     SearchListView<TSearchResult>,
                                                                              doTableView:    SearchTableView<TSearchResult>,
                                                                              linkPrefix?:    SearchResult2Link<TSearchResult>,
                                                                              startView?:     SearchResultsMode,
                                                                              context?:       SearchContext) {

    requestURL = requestURL.indexOf('?') === -1
                    ? requestURL + '?'
                    : requestURL.endsWith('&')
                          ? requestURL
                          : requestURL + '&';

    let   firstSearch              = true;
    let   offset                   = 0;
    let   limit                    = 10;
    let   currentDateFrom:string   = null;
    let   currentDateTo:string     = null;
    let   viewMode                 = startView !== null ? startView : SearchResultsMode.listView;
    const context__                = { Search: Search };
    let   numberOfResults          = 0;
    let   linkURL                  = "";
    let   filteredNumberOfResults  = 0;
    let   totalNumberOfResults     = 0;

    const controlsDiv              = document.    getElementById("controls")              as HTMLDivElement;
    const patternFilter            = controlsDiv. querySelector ("#patternFilterInput")   as HTMLInputElement;
    const takeSelect               = controlsDiv. querySelector ("#takeSelect")           as HTMLSelectElement;
    const searchButton             = controlsDiv. querySelector ("#searchButton")         as HTMLButtonElement;
    const leftButton               = controlsDiv. querySelector ("#leftButton")           as HTMLButtonElement;
    const rightButton              = controlsDiv. querySelector ("#rightButton")          as HTMLButtonElement;

    const dateFilters              = controlsDiv. querySelector ("#dateFilters")          as HTMLDivElement;
    const dateFrom                 = dateFilters?.querySelector ("#dateFromText")         as HTMLInputElement;
    const dateTo                   = dateFilters?.querySelector ("#dateToText")           as HTMLInputElement;
    //const datepicker               = dateFilters !== null ? new DatePicker() : null;

    const listViewButton           = controlsDiv. querySelector ("#listView")             as HTMLButtonElement;
    const tableViewButton          = controlsDiv. querySelector ("#tableView")            as HTMLButtonElement;

    const messageDiv               = document.    getElementById('message')               as HTMLDivElement;
    const localSearchMessageDiv    = document.    getElementById('localSearchMessage')    as HTMLDivElement;
    const searchResultsDiv         = document.    querySelector (".searchResults")        as HTMLDivElement;
    const downLoadButton           = document.    getElementById("downLoadButton")        as HTMLAnchorElement;


    function DoSearchError(Message: string) {

        messageDiv.innerHTML = Message;

        if (downLoadButton)
            downLoadButton.style.display = "none";

    }

    function Search(deletePreviousResults: boolean,
                    resetSkip?:            boolean,
                    whenDone?:             any)
    {

        if (resetSkip)
            offset = 0;

        // handle local searches
        if (patternFilter.value[0] === '#')
        {

            if (whenDone !== null)
                whenDone();

            return;

        }

        // To avoid multiple clicks while waiting for the results from a slow server
        leftButton.disabled   = true;
        rightButton.disabled  = true;

        const filters         = (patternFilter.value !== ""                             ? "&match="   + encodeURI(patternFilter.value) : "") +
                                (searchFilters                                          ? searchFilters() : "") +
                                (currentDateFrom     !=  null && currentDateFrom !== "" ? "&from="    + currentDateFrom                : "") +
                                (currentDateTo       !=  null && currentDateTo   !== "" ? "&to="      + currentDateTo                  : "");

        if (downLoadButton)
            downLoadButton.href = requestURL + "download" + filters;

        OCPIGet(requestURL + filters + "&offset=" + offset + "&limit=" + limit,

                (status, response, httpHeaders) => {

                    try
                    {

                        if (status == 200 && response) {

                            const ocpiResponse = JSON.parse(response) as IOCPIResponse;

                            if (ocpiResponse.status_code >= 1000 &&
                                ocpiResponse.status_code <  2000)
                            {

                                if (ocpiResponse?.data &&
                                    Array.isArray(ocpiResponse.data))
                                {

                                    const searchResults = ocpiResponse.data as Array<TSearchResult>;

                                    numberOfResults          = searchResults.length;

                                    // https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/transport_and_format.md
                                    linkURL                  = httpHeaders("Link");
                                    totalNumberOfResults     = Number.parseInt(httpHeaders("X-Total-Count"));
                                    filteredNumberOfResults  = Number.parseInt(httpHeaders("X-Filtered-Count"));
                                    //limit                    = Number.parseInt(httpHeaders("X-Limit"));

                                    if (Number.isNaN(totalNumberOfResults))
                                        totalNumberOfResults     = numberOfResults;

                                    if (Number.isNaN(filteredNumberOfResults))
                                        filteredNumberOfResults  = totalNumberOfResults;

                                    if (deletePreviousResults)
                                        searchResultsDiv.innerHTML = "";

                                    if (firstSearch && doStartUp) {
                                        //doStartUp(JSONresponse);
                                        firstSearch = false;
                                    }

                                    switch (viewMode)
                                    {

                                        case SearchResultsMode.tableView:
                                            try
                                            {
                                                doTableView(
                                                    searchResults,
                                                    searchResultsDiv
                                                );
                                            }
                                            catch (exception)
                                            {
                                                console.debug("Exception in search table view: " + exception);
                                            }
                                            break;

                                        case SearchResultsMode.listView:
                                            if (searchResults.length > 0) {

                                                let resultCounter = offset + 1;

                                                for (const searchResult of searchResults) {

                                                    try {

                                                        const searchResultAnchor      = searchResultsDiv.appendChild(document.createElement('a')) as HTMLAnchorElement;
                                                        searchResultAnchor.id         = nameOfItem + "_" + idOfItem(searchResult);
                                                        searchResultAnchor.className  = "searchResult " + nameOfItem;

                                                        if (linkPrefix) {

                                                            const prefix = linkPrefix(searchResult);

                                                            if (prefix !== null && prefix.length > 0)
                                                                searchResultAnchor.href = prefix + nameOfItems + "/" + idOfItem(searchResult);

                                                        }

                                                        doListView(
                                                            resultCounter,
                                                            searchResult,
                                                            searchResultAnchor
                                                        );

                                                        resultCounter++;

                                                    }
                                                    catch (exception)
                                                    {
                                                        DoSearchError("Exception in search list view: " + exception);
                                                        //break;
                                                    }

                                                }

                                                if (downLoadButton)
                                                    downLoadButton.style.display = "block";

                                            }
                                            else
                                            {
                                                if (downLoadButton)
                                                    downLoadButton.style.display = "none";
                                            }
                                            break;

                                    }

                                    messageDiv.innerHTML = searchResults.length > 0
                                                               ? "showing results " + (offset + 1) + " - " + (offset + Math.min(searchResults.length, limit)) +
                                                                     " of " + filteredNumberOfResults
                                                               : "no matching " + nameOfItems2 + " found";

                                    if (offset > 0)
                                        leftButton.disabled  = false;

                                    if (offset + limit < filteredNumberOfResults)
                                        rightButton.disabled = false;

                                }
                                else
                                    DoSearchError("Invalid search results!");

                            }
                            else
                                DoSearchError("OCPI Status Code " + ocpiResponse.status_code + (ocpiResponse.status_message ? ": " + ocpiResponse.status_message : ""));

                        }
                        else
                            DoSearchError("HTTP Status Code " + status + (response ? ": " + response : ""));

                    }
                    catch (exception)
                    {
                        DoSearchError("Exception occurred: " + exception);
                    }

                    if (whenDone)
                        whenDone();

                },

                (status, response, httpHeaders) => {

                    DoSearchError("Server error: " + status + "<br />" + response);

                    if (whenDone)
                        whenDone();

                });

    }


    if (patternFilter !== null)
    {

        patternFilter.onchange = () => {
            if (patternFilter.value[0] !== '#') {
                offset = 0;
            }
        }

        patternFilter.onkeyup = (ev: KeyboardEvent) => {

            if (patternFilter.value[0] !== '#') {
                if (ev.key === 'Enter')
                    Search(true);
            }

            // Client-side searches...
            else
            {

                const pattern          = patternFilter.value.substring(1);
                const logLines         = Array.from(document.getElementById('searchResults').getElementsByClassName('searchResult')) as HTMLDivElement[];
                let   numberOfMatches  = 0;

                for (const logLine of logLines) {

                    if (logLine.innerHTML.indexOf(pattern) > -1) {
                        logLine.style.display = 'block';
                        numberOfMatches++;
                    }

                    else
                        logLine.style.display = 'none';

                }

                if (localSearchMessageDiv !== null) {

                    localSearchMessageDiv.innerHTML = numberOfMatches > 0
                                                          ? numberOfMatches + " local matches"
                                                          : "no matching " + nameOfItems2 + " found";

                }

            }

        }

    }

    limit = parseInt(takeSelect.options[takeSelect.selectedIndex].value);
    takeSelect.onchange = () => {
        limit = parseInt(takeSelect.options[takeSelect.selectedIndex].value);
        Search(true);
    }

    searchButton.onclick = () => {
        Search(true);
    }

    leftButton.disabled = true;
    leftButton.onclick = () => {

        leftButton.classList.add("busy", "busyActive");
        rightButton.classList.add("busy");

        offset -= limit;

        if (offset < 0)
            offset = 0;

        Search(true, false, () => {
            leftButton.classList.remove("busy", "busyActive");
            rightButton.classList.remove("busy");
        });

    }

    rightButton.disabled = true;
    rightButton.onclick = () => {

        leftButton.classList.add("busy");
        rightButton.classList.add("busy", "busyActive");

        offset += limit;

        Search(true, false, () => {
            leftButton.classList.remove("busy");
            rightButton.classList.remove("busy", "busyActive");
        });

    }

    document.onkeydown = (ev: KeyboardEvent) => {

        if (ev.key === 'ArrowLeft' || ev.key === 'ArrowUp') {
            if (leftButton.disabled === false)
                leftButton.click();
            return;
        }

        if (ev.key === 'ArrowRight' || ev.key === 'ArrowDown') {
            if (rightButton.disabled === false)
                rightButton.click();
            return;
        }

        if (ev.key === 'Home') {
            // Will set skip = 0!
            Search(true, true);
            return;
        }

        if (ev.key === 'End') {
            offset = Math.trunc(filteredNumberOfResults / limit) * limit;
            Search(true, false);
            return;
        }

    }

    //if (dateFrom !== null) {
    //    dateFrom.onclick = () => {
    //        datepicker.show(dateFrom,
    //            currentDateFrom,
    //            function (newDate) {
    //                dateFrom.value = parseUTCDate(newDate);
    //                currentDateFrom = newDate;
    //                Search(true, true);
    //            });
    //    }
    //}

    //if (dateTo !== null) {
    //    dateTo.onclick = () => {
    //        datepicker.show(dateTo,
    //            currentDateTo,
    //            function (newDate) {
    //                dateTo.value = parseUTCDate(newDate);
    //                currentDateTo = newDate;
    //                Search(true, true);
    //            });
    //    }
    //}

    if (listViewButton !== null) {
        listViewButton.onclick = () => {
            viewMode = SearchResultsMode.listView;
            Search(true);
        }
    }

    if (tableViewButton !== null) {
        tableViewButton.onclick = () => {
            viewMode = SearchResultsMode.tableView;
            Search(true);
        }
    }


    if (context)
        context(context__);

    Search(true);

    return context__;

}


async function OCPIGetCollection<TMetadata extends TMetadataDefaults, TSearchResult>(requestURL:    string,
                                                                                     doStartUp:     SearchStartUp<TMetadata>,
                                                                                     nameOfItems:   string,
                                                                                     doStatistics:  StatisticsDelegate<TSearchResult>,
                                                                                     doFinish?:     StatisticsFinishedDelegate<TSearchResult>) {

    requestURL = requestURL.indexOf('?') === -1
                    ? requestURL + '?'
                    : requestURL.endsWith('&')
                          ? requestURL
                          : requestURL + '&';

    let   firstSearch              = true;
    let   offset                   = 0;
    let   limit                    = 2500;
    let   currentDateFrom:string   = null;
    let   currentDateTo:string     = null;
    let   numberOfResults          = 0;
    let   linkURL                  = "";
    let   filteredNumberOfResults  = 0;
    let   totalNumberOfResults     = 0;

    const messageDiv               = document.getElementById('message')         as HTMLDivElement;
    const downLoadButton           = document.getElementById("downLoadButton")  as HTMLAnchorElement;

    function DoSearchError(Message: string) {

        messageDiv.innerHTML = Message;

        if (downLoadButton)
            downLoadButton.style.display = "none";

    }


    offset = 0;

    const filters = (currentDateFrom !=  null && currentDateFrom !== "" ? "&from="    + currentDateFrom                : "") +
                    (currentDateTo   !=  null && currentDateTo   !== "" ? "&to="      + currentDateTo                  : "");

    if (downLoadButton)
        downLoadButton.href = requestURL + "download" + filters;


    do {

        const [ocpiResponse, httpHeaders] = await OCPIGetAsync(requestURL + "offset=" + offset + "&limit=" + limit);

        if (ocpiResponse.data &&
            Array.isArray(ocpiResponse.data) &&
            ocpiResponse.data.length > 0) {

            const searchResults = ocpiResponse.data as Array<TSearchResult>;

            numberOfResults = searchResults.length;

            // https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/transport_and_format.md
            linkURL                  = httpHeaders("Link");
            totalNumberOfResults     = Number.parseInt(httpHeaders("X-Total-Count"));
            filteredNumberOfResults  = Number.parseInt(httpHeaders("X-Filtered-Count"));
            //limit                    = Number.parseInt(httpHeaders("X-Limit"));

            if (Number.isNaN(totalNumberOfResults))
                totalNumberOfResults    = numberOfResults;

            if (Number.isNaN(filteredNumberOfResults))
                filteredNumberOfResults = totalNumberOfResults;

            if (searchResults.length > 0) {

                let resultCounter = offset + 1;

                for (const searchResult of searchResults) {

                    try {

                        doStatistics(
                            resultCounter,
                            searchResult
                        );

                        resultCounter++;

                    }
                    catch (exception) {
                        DoSearchError("Exception in statistics delegate: " + exception);
                        //break;
                    }

                }

            }

        }

        offset = offset + numberOfResults;

    } while (offset + numberOfResults < totalNumberOfResults);

    if (doFinish)
        doFinish(totalNumberOfResults);

    //if (downLoadButton)
    //    downLoadButton.style.display = "block";



    //OCPIGet(
    //
    //    requestURL + "offset=" + offset + "&limit=" + limit,
    //
    //    (status, response, httpHeaders) => {
    //
    //        try
    //        {
    //
    //            var xLimit = 0;
    //
    //            if (status == 200 && response) {
    //
    //                const ocpiResponse = JSON.parse(response) as IOCPIResponse;
    //
    //                if (ocpiResponse.status_code >= 1000 &&
    //                    ocpiResponse.status_code <  2000)
    //                {
    //
    //                    if (ocpiResponse?.data               &&
    //                        Array.isArray(ocpiResponse.data) &&
    //                        ocpiResponse.data.length > 0)
    //                    {
    //
    //                        xLimit = Number.parseInt(httpHeaders("X-Limit"));
    //
    //                        if (isNaN(xLimit))
    //                            xLimit = ocpiResponse?.data.length;
    //
    //                        const searchResults = ocpiResponse.data as Array<TSearchResult>;
    //
    //                        numberOfResults          = searchResults.length;
    //
    //                        // https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/transport_and_format.md
    //                        linkURL                  = httpHeaders("Link");
    //                        totalNumberOfResults     = Number.parseInt(httpHeaders("X-Total-Count"));
    //                        filteredNumberOfResults  = Number.parseInt(httpHeaders("X-Filtered-Count"));
    //                        //limit                    = Number.parseInt(httpHeaders("X-Limit"));
    //
    //                        if (Number.isNaN(totalNumberOfResults))
    //                            totalNumberOfResults     = numberOfResults;
    //
    //                        if (Number.isNaN(filteredNumberOfResults))
    //                            filteredNumberOfResults  = totalNumberOfResults;
    //
    //
    //                        //if (deletePreviousResults || numberOfResults > 0)
    //                        //    searchResultsDiv.innerHTML = "";
    //
    //                        if (firstSearch && doStartUp) {
    //                            //doStartUp(JSONresponse);
    //                            firstSearch = false;
    //                        }
    //
    //                        if (searchResults.length > 0) {
    //
    //                            let resultCounter = offset + 1;
    //
    //                            for (const searchResult of searchResults) {
    //
    //                                try {
    //
    //                                    doStatistics(
    //                                        resultCounter,
    //                                        searchResult
    //                                    );
    //
    //                                    resultCounter++;
    //
    //                                }
    //                                catch (exception)
    //                                {
    //                                    DoSearchError("Exception in statistics delegate: " + exception);
    //                                    //break;
    //                                }
    //
    //                            }
    //
    //                            if (downLoadButton)
    //                                downLoadButton.style.display = "block";
    //
    //                        }
    //
    //                        //messageDiv.innerHTML = searchResults.length > 0
    //                        //                           ? "showing results " + (offset + 1) + " - " + (offset + Math.min(searchResults.length, limit)) +
    //                        //                                 " of " + filteredNumberOfResults
    //                        //                           : "no matching " + nameOfItems + " found";
    //
    //                    }
    //                    else
    //                        DoSearchError("Invalid search results!");
    //
    //                }
    //                else
    //                    DoSearchError("OCPI Status Code " + ocpiResponse.status_code + (ocpiResponse.status_message ? ": " + ocpiResponse.status_message : ""));
    //
    //            }
    //            else
    //                DoSearchError("HTTP Status Code " + status + (response ? ": " + response : ""));
    //
    //        }
    //        catch (exception)
    //        {
    //            DoSearchError("Exception occurred: " + exception);
    //        }
    //
    //        if (doFinish)
    //            doFinish(totalNumberOfResults);
    //
    //    },
    //
    //    (status, response, httpHeaders) => {
    //
    //      DoSearchError("Server error: " + status + "<br />" + response);
    //
    //  }
    //
    //);

}


function GetDefaults() : ICommons
{
    return {
        topLeft:            document.getElementById("topLeft")            as HTMLDivElement,
        menuVersions:       document.getElementById("menuVersions")       as HTMLAnchorElement,
        menuRemoteParties:  document.getElementById("menuRemoteParties")  as HTMLAnchorElement
    }
}



function CreateProperty(parent: HTMLDivElement | HTMLAnchorElement, className: string, key: string, innerHTML: string | HTMLDivElement): HTMLDivElement {

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



// Legacies!!!

// #region HTTP(Method, RessourceURI, Data, OnSuccess, OnError)

function HTTP(Method:       string,
              RessourceURI: string,
              Data,
              OnSuccess,
              OnError) {

    // #region Make HTTP call

    const ajax = new XMLHttpRequest();
    ajax.open(Method, RessourceURI, true);
    ajax.setRequestHeader("Accept",       "application/json; charset=UTF-8");
    ajax.setRequestHeader("Content-Type", "application/json; charset=UTF-8");

    //if (APIKey !== null)
    //    ajax.setRequestHeader("APIKey", APIKey);

    ajax.onreadystatechange = function () {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            // Ok
            if (this.status >= 100 && this.status < 300) {

                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));

                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(this.status, ajax.responseText);

            }

            else
                if (OnError && typeof OnError === 'function')
                    OnError(this.status, this.statusText, ajax.responseText);

        }

    }

    if (Data !== null)
        ajax.send(JSON.stringify(Data));
    else
        ajax.send();

    // #endregion

}

// #endregion

// #region HTTPGet  (RessourceURI, OnSuccess, OnError)

function HTTPGet(RessourceURI: string,
                 OnSuccess,
                 OnError) {

    // #region Make HTTP call

    const ajax = new XMLHttpRequest();
    ajax.open("GET", RessourceURI, true);
    ajax.setRequestHeader("Accept",   "application/json; charset=UTF-8");
    ajax.setRequestHeader("X-Portal", "true");
    //ajax.setRequestHeader("Content-Type", "application/json; charset=UTF-8");

    //if (APIKey !== null)
    //    ajax.setRequestHeader("APIKey", APIKey);

    ajax.onreadystatechange = function () {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            // Ok
            if (this.status >= 100 && this.status < 300) {

                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));

                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(this.status, ajax.responseText);

            }

            else
                if (OnError && typeof OnError === 'function')
                    OnError(this.status, this.statusText, ajax.responseText);

        }

    }

    ajax.send();

    // #endregion

}

// #endregion

function ParseJSON_LD<T>(Text: string,
    Context: string = null): T {

    const data = JSON.parse(Text);

    if (!Array.isArray(data))
        data["id"] = data["@id"];

    return data as T;

}
