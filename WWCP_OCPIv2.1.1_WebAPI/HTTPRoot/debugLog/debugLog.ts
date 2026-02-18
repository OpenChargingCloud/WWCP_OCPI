///<reference path="../../../WWCP_OCPI_Common_WebAPI/HTTPRoot/ts/date.format.ts" />

function StartEventsSSE() {

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

    function CreateLogEntry(timestamp, roamingNetwork, eventTrackingId, command, message, connectionColorKey) {

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
                                "<div class=\"command\">"         + command         + "</div>" +
                                "<div class=\"message\">"         + message.reduce(function (a, b) { return a + "<br />" + b; }); + "</div>";

        if (div.innerHTML.indexOf(streamFilterInput.value) > -1)
            div.style.display = 'table-row';
        else
            div.style.display = 'none';

        eventsDiv.insertBefore(div, eventsDiv.firstChild);

    }

    function AppendLogEntry(timestamp, roamingNetwork, command, searchPattern, message) {

        const allLogLines = eventsDiv.getElementsByClassName('logLine');

        for (let i = 0; i < allLogLines.length; i++) {
            if (allLogLines[i].getElementsByClassName("command")[0].innerHTML == command) {
                if (allLogLines[i].innerHTML.indexOf(searchPattern) > -1) {
                    allLogLines[i].getElementsByClassName("message")[0].innerHTML += message;
                    break;
                }
            }
        }

    }



    const eventSource = window.EventSource !== undefined
                            ? new EventSource('/sse1')
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
                    "sub1",
                    container.outerHTML,
                    request.EVSEId           ?? "" // ConnectionColorKey
                );

            }
            catch (exception) {
                console.error(exception);
            }

        }, false);

        eventSource.addEventListener('AUTHSTARTResponse',               function (event) {

            try
            {

                const response = JSON.parse((event as MessageEvent).data);
                const result   = response.result;

                AppendLogEntry(response.timestamp,
                               response.roamingNetwork,
                               // 1) Search for a logline with this command
                               "AUTHSTART",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + response.eventTrackingId,

                               " &rArr; " + result.result +
                               (result.description          ? " '"                                        + result.description["eng"]   + "'"      : "") +
                               (result.providerId           ? " by <div class=\"providerId\">"            + result.providerId           + "</div>" : "") +
                               (result.authorizatorId       ? " via <div class=\"authorizatorId\">"       + result.authorizatorId       + "</div>" : "") +
                               (result.EMPRoamingProviderId ? " via <div class=\"EMPRoamingProviderId\">" + result.EMPRoamingProviderId + "</div>" : "") +
                               (result.CSORoamingProviderId ? " via <div class=\"CSORoamingProviderId\">" + result.CSORoamingProviderId + "</div>" : "") +
                               (result.sessionId            ? " <a href=\"../RNs/" + response.roamingNetworkId + "/chargingSessions/" + result.sessionId + "\" class=\"sessionId\"><i class=\"fas fa-file-contract\"></i></a>" : "") +
                                " [" + response.runtime + " ms]");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


    }

}
