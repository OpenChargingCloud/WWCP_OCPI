///<reference path="../../../WWCP_OCPI_Common_WebAPI/HTTPRoot/ts/date.format.ts" />

function StartDebugLog() {

    const connectionColors   = {};
    const eventsDiv          = document.getElementById('eventsDiv');
    const streamFilterInput  = document.getElementById('eventsFilterDiv').getElementsByTagName('input')[0] as HTMLInputElement;
    streamFilterInput.onchange = () => {

        const allLogLines = eventsDiv.getElementsByClassName('logLine') as HTMLCollectionOf<HTMLDivElement>;

        for (let i = 0; i < allLogLines.length; i++) {
            if (allLogLines[i].innerHTML.indexOf(streamFilterInput.value) > -1)
                allLogLines[i].style.display = 'table-row';
            else
                allLogLines[i].style.display = 'none';
        }

    }

    function GetConnectionColors(connectionId) {

        const colors = connectionColors[connectionId];

        if (colors !== undefined)
            return colors;

        else
        {

            const red   = Math.floor(Math.random() * 80 + 165).toString(16);
            const green = Math.floor(Math.random() * 80 + 165).toString(16);
            const blue  = Math.floor(Math.random() * 80 + 165).toString(16);

            const connectionColor = red + green + blue;

            connectionColors[connectionId]             = new Object();
            connectionColors[connectionId].textcolor   = "000000";
            connectionColors[connectionId].background  = connectionColor;

            return connectionColors[connectionId];

        }

    }

    function CreateLogEntry(timestamp, roamingNetwork, eventTrackingId, from, to, command, message, connectionColorKey) {

        const connectionColor = GetConnectionColors(connectionColorKey);

        if (typeof message === 'string') {
            message = [message];
        }

        const div = document.createElement('div');
        div.className         = "logLine";
        div.style.color       = "#" + connectionColor.textcolor;
        div.style.background  = "#" + connectionColor.background;
        div.innerHTML         = "<div class=\"timestamp\">"       + new Date(timestamp).format('dd.mm.yyyy HH:MM:ss') + "</div>" +
                                "<div class=\"roamingNetwork\">"  + roamingNetwork  + "</div>" +
                                "<div class=\"eventTrackingId\">" + eventTrackingId + "</div>" +
                                "<div class=\"from\">"            + (from ?? "")    + "</div>" +
                                "<div class=\"to\">"              + (to   ?? "")    + "</div>" +
                                "<div class=\"command\">"         + command + "</div>" +
                                "<div class=\"message\">"         + message.reduce(function (a, b) { return a + "<br />" + b; }); + "</div>";

        if (div.innerHTML.indexOf(streamFilterInput.value) > -1)
            div.style.display = 'table-row';
        else
            div.style.display = 'none';

        eventsDiv.insertBefore(div, eventsDiv.firstChild);

    }

    function AppendLogEntry(timestamp, roamingNetwork, eventTrackingId, message, runtime) {

        const searchPattern  = "\"eventTrackingId\">" + eventTrackingId;
        const allLogLines    = eventsDiv.getElementsByClassName('logLine');

        for (let i = 0; i < allLogLines.length; i++) {
            if (allLogLines[i].innerHTML.indexOf(searchPattern) > -1) {
                allLogLines[i].getElementsByClassName("message")[0].innerHTML += `${message} [${runtime} ms]`;
                break;
            }
        }

    }



    const eventSource = window.EventSource !== undefined
                            ? new EventSource('debugLog')
                            : null;

    if (eventSource !== null)
    {

        // Will only be called for events without an event type!
        eventSource.onmessage = function (event) {
            console.debug(event);
        };

        eventSource.onerror = function (event) {
            console.debug(event);
        };



        eventSource.addEventListener('sub1', (event: MessageEvent) => {

            try
            {

                const request         = JSON.parse(event.data);

                const entries         = Object.entries(request);
                if (entries.length === 0)
                    return;

                const [key, value]    = entries[0];

                const container       = document.createElement('div');
                container.className   = 'sub1';

                const keyDiv          = document.createElement('div');
                keyDiv.className      = 'key';
                keyDiv.textContent    = String(key);

                const valueDiv        = document.createElement('div');
                valueDiv.className    = 'value';
                valueDiv.textContent  = value == null ? '' : String(value);

                container.append(keyDiv, valueDiv);


                CreateLogEntry(
                    request.timestamp        ?? Date.now(),
                    request.roamingNetworkId ?? "",
                    request.eventTrackingId  ?? "",
                    "",
                    "",
                    "sub1",
                    container.outerHTML,
                    request.EVSEId           ?? "" // ConnectionColorKey
                );

            }
            catch (exception) {
                console.error(exception);
            }

        }, false);




        eventSource.addEventListener('OnPutSessionRequest', (event: MessageEvent<string>) => {

            try
            {

                const request = JSON.parse(event.data);

                const entries = Object.entries(request);
                if (entries.length === 0)
                    return;

                CreateLogEntry(
                    request.timestamp ?? Date.now(),
                    request.roamingNetworkId ?? "",
                    request.eventTrackingId ?? "",
                    request.from,
                    request.to,
                    "OnPutSessionRequest",
                    `${request.id} (${request.status}, ${request.kwh}, ${request.total_cost} ${request.currency})`,
                    request.from ?? "" // ConnectionColorKey
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPutSessionRequest',
                    event.data,
                    exception
                );
            }

        }, false);

        eventSource.addEventListener('OnPutSessionResponse', (event: MessageEvent<string>) => {

            try
            {

                const response = JSON.parse(event.data);

                AppendLogEntry(
                    response.timestamp,
                    response.roamingNetwork,
                    response.eventTrackingId,
                    `⇒ !`,
                    response.runtime
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPutSessionResponse',
                    event.data,
                    exception
                );
            }

        }, false);

        eventSource.addEventListener('OnPatchSessionRequest', (event: MessageEvent<string>) => {

            try
            {

                const request = JSON.parse(event.data);

                const entries = Object.entries(request);
                if (entries.length === 0)
                    return;

                CreateLogEntry(
                    request.timestamp ?? Date.now(),
                    request.roamingNetworkId ?? "",
                    request.eventTrackingId ?? "",
                    request.from,
                    request.to,
                    "OnPatchSessionRequest",
                    `${request.sessionId}`,
                    request.from ?? "" // ConnectionColorKey
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPatchSessionRequest',
                    event.data,
                    exception
                );
            }

        }, false);

        eventSource.addEventListener('OnPatchSessionResponse', (event: MessageEvent<string>) => {

            try
            {

                const response = JSON.parse(event.data);

                AppendLogEntry(
                    response.timestamp,
                    response.roamingNetwork,
                    response.eventTrackingId,
                    `⇒ !`,
                    response.runtime
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPatchSessionResponse',
                    event.data,
                    exception
                );
            }

        }, false);


        eventSource.addEventListener('OnPostCDRRequest', (event: MessageEvent<string>) => {

            try
            {

                const request = JSON.parse(event.data);

                const entries = Object.entries(request);
                if (entries.length === 0)
                    return;

                CreateLogEntry(
                    request.timestamp        ?? Date.now(),
                    request.roamingNetworkId ?? "",
                    request.eventTrackingId  ?? "",
                    request.from,
                    request.to,
                    "OnPostCDRRequest",
                    `${request.id} (${request.session_id}, ${request.cdr_token})`,
                    request.from ?? "" // ConnectionColorKey
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPostCDRRequest',
                    event.data,
                    exception
                );
            }

        }, false);

        eventSource.addEventListener('OnPostCDRResponse', (event: MessageEvent<string>) => {

            try
            {

                const response = JSON.parse(event.data);

                AppendLogEntry(
                    response.timestamp,
                    response.roamingNetwork,
                    response.eventTrackingId,
                    `⇒ ${response.cdrLocation}`,
                    response.runtime
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPostCDRResponse',
                    event.data,
                    exception
                );
            }

        }, false);


        eventSource.addEventListener('OnPostTokenRequest', (event: MessageEvent<string>) => {

            try
            {

                const request = JSON.parse(event.data);

                const entries = Object.entries(request);
                if (entries.length === 0)
                    return;

                CreateLogEntry(
                    request.timestamp ?? Date.now(),
                    request.roamingNetworkId ?? "",
                    request.eventTrackingId ?? "",
                    request.from,
                    request.to,
                    "OnPostTokenRequest",
                    `${request.tokenId} (${request.requestedTokenType})`,
                    request.from ?? "" // ConnectionColorKey
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPostTokenRequest',
                    event.data,
                    exception
                );
            }

        }, false);

        eventSource.addEventListener('OnPostTokenResponse', (event: MessageEvent<string>) => {

            try
            {

                const response = JSON.parse(event.data);

                AppendLogEntry(
                    response.timestamp,
                    response.roamingNetwork,
                    response.eventTrackingId,
                    `⇒ ${response.result.allowed}`,
                    response.runtime
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPostTokenResponse',
                    event.data,
                    exception
                );
            }

        }, false);


    }


    function ShowHTTPSSEError(command:    string,
                              data:       any,
                              exception:  any) {

        const e2 = exception instanceof Error
                       ? exception
                       : new Error(String(exception));

        CreateLogEntry(
            Date.now(),
            "",
            "",
            "",
            "",
            "Error",
            `${command} (${JSON.stringify(data)}) ⇒ ${e2}`,
            "" // ConnectionColorKey
        );

        console.debug(command);
        console.debug(data);
        console.debug(e2);

    }

}
