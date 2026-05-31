///<reference path="../../../WWCP_OCPI_Common_WebAPI/HTTPRoot/ts/date.format.ts" />

function StartDebugLog() {

    const connectionColors = {};
    const eventsDiv = document.getElementById('eventsDiv');
    const streamFilterInput = document.getElementById('eventsFilterDiv').getElementsByTagName('input')[0] as HTMLInputElement;

    // Live filtering as you type...
    streamFilterInput.oninput = () => {

        compileFilter(streamFilterInput.value);

        const allLogLines = Array.from(eventsDiv.getElementsByClassName('logLine') as HTMLCollectionOf<HTMLDivElement>);

        for (let i = 0; i < allLogLines.length; i++) {
            allLogLines[i].style.display =
                matchesFilter(allLogLines[i].innerHTML)
                    ? 'table-row'
                    : 'none';
        }

    };


    const clearButton = document.getElementById('clearEventsButton');
    clearButton.onclick = () => {
        eventsDiv.innerHTML = '';
    };

    // ── Filter help button & panel ──────────────────────────────────

    const filterHelpPanel = document.getElementById('filterHelpPanel');
    const filterHelpButton = document.getElementById('filterHelpButton');
    filterHelpButton.onclick = () => {
        filterHelpPanel.classList.toggle('visible');
    };



    // ── Filter expression engine ──────────────────────────────────────
    function tokenizeFilter(input: string | any[]) {

        const tokens = [];
        let i = 0;

        while (i < input.length) {

            const ch = input[i];

            // Skip whitespace
            if (ch === ' ' || ch === '\t') { i++; continue; }

            // Single-character operators
            if (ch === '(') { tokens.push({ type: 'LPAREN' }); i++; continue; }
            if (ch === ')') { tokens.push({ type: 'RPAREN' }); i++; continue; }
            if (ch === '&') { tokens.push({ type: 'AND' }); i++; continue; }
            if (ch === '|') { tokens.push({ type: 'OR' }); i++; continue; }
            if (ch === '!') { tokens.push({ type: 'NOT' }); i++; continue; }

            // Quoted string: "..."
            if (ch === '"') {
                let str = '';
                i++; // skip opening quote
                while (i < input.length && input[i] !== '"') {
                    str += input[i];
                    i++;
                }
                i++; // skip closing quote
                tokens.push({ type: 'STRING', value: str });
                continue;
            }

            // Regex literal: /pattern/flags
            if (ch === '/') {
                let pattern = '';
                i++; // skip opening /
                while (i < input.length && input[i] !== '/') {
                    if (input[i] === '\\\\' && i + 1 < input.length) {
                        pattern += input[i] + input[i + 1];
                        i += 2;
                    } else {
                        pattern += input[i];
                        i++;
                    }
                }
                i++; // skip closing /
                let flags = '';
                while (i < input.length && /[gimsuy]/.test(input[i])) {
                    flags += input[i];
                    i++;
                }
                tokens.push({ type: 'REGEX', pattern, flags });
                continue;
            }

            // Bare word (unquoted substring) — everything until a special char or whitespace
            let word = '';
            while (i < input.length && !'(&|)! \t"'.includes(input[i])) {
                word += input[i];
                i++;
            }
            if (word.length > 0) {
                tokens.push({ type: 'STRING', value: word });
            }

        }

        return tokens;

    }

    function parseFilter(input: string): FilterAST {

        // An empty filter matches everything
        const trimmed = input.trim();
        if (trimmed === '')
            return { type: 'TRUE' };

        let pos = 0;
        const tokens = tokenizeFilter(trimmed);

        // Strip trailing binary operators (& / |) that have no right operand
        while (tokens.length > 0 &&
            (tokens[tokens.length - 1].type === 'AND' ||
                tokens[tokens.length - 1].type === 'OR')) {
            tokens.pop();
        }

        // Match everything, when stripping left us with no tokens
        if (tokens.length === 0)
            return { type: 'TRUE' };


        function peek() {

            return pos < tokens.length
                ? tokens[pos]
                : null;

        }

        function consume(expectedType: string) {

            const t = tokens[pos];

            if (expectedType && (!t || t.type !== expectedType))
                throw new Error(`Expected ${expectedType} at position ${pos}`);

            pos++;
            return t;

        }

        // Grammar (precedence low → high):
        //   expr     = andExpr ( '|' andExpr )*
        //   andExpr  = notExpr ( '&' notExpr )*
        //   notExpr  = '!' notExpr | primary
        //   primary  = '(' expr ')' | STRING | REGEX
        function parseExpr() {
            let left = parseAndExpr();
            while (peek() && peek().type === 'OR') {
                consume('OR');
                const right = parseAndExpr();
                left = { type: 'OR', left, right };
            }
            return left;
        }

        function parseAndExpr() {

            let left = parseNotExpr();

            while (peek() && peek().type === 'AND') {
                consume('AND');
                const right = parseNotExpr();
                left = { type: 'AND', left, right };
            }

            return left;

        }

        function parseNotExpr() {

            if (peek() && peek().type === 'NOT') {
                consume('NOT');
                const operand = parseNotExpr(); // right-recursive for !!x
                return { type: 'NOT', operand };
            }

            return parsePrimary();

        }

        function parsePrimary() {

            const t = peek();

            if (!t)
                throw new Error('Unexpected end of filter expression');

            if (t.type === 'LPAREN') {
                consume('LPAREN');
                const expr = parseExpr();
                consume('RPAREN');
                return expr;
            }

            if (t.type === 'STRING') {
                consume('STRING');
                return { type: 'SUBSTR', value: t.value.toLowerCase() };
            }

            if (t.type === 'REGEX') {
                consume('REGEX');
                return { type: 'REGEX', regex: new RegExp(t.pattern, t.flags) };
            }

            throw new Error(`Unexpected token: ${t.type}`);

        }

        const ast = parseExpr();

        if (pos < tokens.length)
            throw new Error(`Unexpected token at position ${pos}: ${tokens[pos].type}`);

        return ast;

    }

    function evalFilter(ast: FilterAST, text: string): boolean {
        switch (ast.type) {
            case 'TRUE': return true;
            case 'SUBSTR': return text.toLowerCase().includes(ast.value);
            case 'REGEX': return ast.regex.test(text);
            case 'NOT': return !evalFilter(ast.operand, text);
            case 'AND': return evalFilter(ast.left, text) && evalFilter(ast.right, text);
            case 'OR': return evalFilter(ast.left, text) || evalFilter(ast.right, text);
            default: return true;
        }
    }

    // Compile once, evaluate many times
    type FilterAST =
        | { type: 'TRUE' }
        | { type: 'SUBSTR'; value: string }
        | { type: 'REGEX'; regex: RegExp }
        | { type: 'NOT'; operand: FilterAST }
        | { type: 'AND'; left: FilterAST; right: FilterAST }
        | { type: 'OR'; left: FilterAST; right: FilterAST };

    let currentFilterAST: FilterAST = { type: 'TRUE' };

    function compileFilter(filterString: string) {
        try {

            currentFilterAST = parseFilter(filterString);

        } catch (e) {

            console.warn('Invalid filter expression:', e instanceof Error ? e.message : String(e));

            // On syntax error, fall back to simple substring match
            const val = filterString.trim().toLowerCase();
            currentFilterAST = val === ''
                ? { type: 'TRUE' }
                : { type: 'SUBSTR', value: val };

        }
    }

    function matchesFilter(innerHTML: string) {
        return evalFilter(currentFilterAST, innerHTML);
    }




    // ── Settings button & panel ─────────────────────────────────────
    const settingsPanel       = document.getElementById('settingsPanel');
    const settingsButton      = document.getElementById('settingsButton');
    const maxEventsInput      = document.getElementById('maxEventsInput') as HTMLInputElement;

    let max_number_of_events  = parseInt(localStorage.getItem('max_number_of_events')) || 500;
    maxEventsInput.value      = max_number_of_events.toString();

    settingsButton.onclick    = () => {
        settingsPanel.classList.toggle('visible');
    };

    maxEventsInput.oninput = () => {

        const val = parseInt(maxEventsInput.value);

        if (!isNaN(val) && val >= 10) {
            max_number_of_events = val;
            localStorage.setItem('max_number_of_events', val.toString());
            trimOldEvents();
        }

    };


    function trimOldEvents() {

        const logLines = eventsDiv.getElementsByClassName('logLine') as HTMLCollectionOf<HTMLDivElement>;

        // First pass: remove oldest events that are currently hidden by the filter
        if (logLines.length > max_number_of_events) {
            for (let i = logLines.length - 1; i >= 0 && logLines.length > max_number_of_events; i--) {
                if (logLines[i].style.display === 'none')
                    eventsDiv.removeChild(logLines[i]);
            }
        }

        // Second pass: if still over the limit, remove the oldest visible events
        while (logLines.length > max_number_of_events) {
            // New events are inserted at the top,
            // so the oldest events are at the bottom
            eventsDiv.removeChild(eventsDiv.lastElementChild);
        }

    }

    const eventsObserver = new MutationObserver((mutations) => {
        for (const mutation of mutations) {
            if (mutation.type === 'childList' && mutation.addedNodes.length > 0)
                trimOldEvents();
        }
    });

    eventsObserver.observe(
        eventsDiv,
        { childList: true }
    );






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

    function CreateLogEntry(timestamp, eventTrackingId, remotePartyId, from, to, direction, command, message, connectionColorKey) {

        const connectionColor = GetConnectionColors(connectionColorKey);

        if (typeof message === 'string') {
            message = [message];
        }

        const div = document.createElement('div');
        div.className         = "logLine";
        div.style.color       = "#" + connectionColor.textcolor;
        div.style.background  = "#" + connectionColor.background;
        div.innerHTML         = "<div class=\"timestamp\">"       + new Date(timestamp).format('dd.mm.yyyy HH:MM:ss') + "</div>" +
                                "<div class=\"eventTrackingId\">" + eventTrackingId + "</div>" +
                                "<div class=\"remotePartyId\">"   + remotePartyId   + "</div>" +
                                "<div class=\"from\">"            + (from ?? "")    + "</div>" +
                                "<div class=\"to\">"              + (to   ?? "")    + "</div>" +
                                "<div class=\"direction\">"       + (direction == "in" ? "⇐" : direction == "out" ? "⇒" : "") + "</div>" +
                                "<div class=\"command\">"         + command + "</div>" +
                                "<div class=\"message\">"         + message.reduce(function (a: string, b: string) { return a + "<br />" + b; }) + "</div>" +
                                "<div class=\"runtime\"></div>";

        div.style.display = matchesFilter(div.innerHTML)
            ? 'table-row'
            : 'none';

        eventsDiv.insertBefore(
            div,
            eventsDiv.firstChild
        );

    }

    function AppendLogEntry(timestamp:        any,
                            roamingNetwork:   any,
                            eventTrackingId:  string,
                            message:          string,
                            runtime:          any)
    {

        const searchPattern  = "\"eventTrackingId\">" + eventTrackingId;
        const allLogLines    = eventsDiv.getElementsByClassName('logLine');

        for (let i = 0; i < allLogLines.length; i++) {
            if (allLogLines[i].innerHTML.indexOf(searchPattern) > -1) {
                allLogLines[i].getElementsByClassName("message")[0].innerHTML += `${message}`;
                allLogLines[i].getElementsByClassName("runtime")[0].innerHTML += `${runtime} ms`;
                break;
            }
        }

    }



    function multiLanguageTextToHTML(items: { language: string; text: string }[]): string
    {

        if (items.length === 0)
            return "";

        return `
          <ul class="multilanguage">
            ${items.map(item => `
                <li lang="${item.language}">
                  <strong>[${item.language.toUpperCase()}]</strong> 
                  ${escapeHtml(item.text)}
                </li>
              `).join("\n")}
          </ul>
        `.trim();

    }

    function escapeHtml(unsafe: string): string {
        return unsafe
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }


    const eventSource = window.EventSource !== undefined
                            ? new EventSource('debugLog')
                            : null;

    if (eventSource !== null)
    {

        // Will only be called for events without an event type!
        eventSource.onmessage = function (event) {

            try
            {

                const request = JSON.parse(event.data);

                const entries = Object.entries(request);
                if (entries.length === 0)
                    return;

                const [key, value] = entries[0];

                const container = document.createElement('div');
                container.className = 'OnMessage';

                const keyDiv = document.createElement('div');
                keyDiv.className = 'key';
                keyDiv.textContent = String(key);

                const valueDiv = document.createElement('div');
                valueDiv.className = 'value';
                valueDiv.textContent = value == null ? '' : String(value);

                container.append(keyDiv, valueDiv);


                CreateLogEntry(
                    request.timestamp ?? Date.now(),
                    request.roamingNetworkId ?? "",
                    request.eventTrackingId ?? "",
                    "",
                    "",
                    "",
                    "OnMessage",
                    container.outerHTML,
                    request.EVSEId ?? "" // ConnectionColorKey
                );

            }
            catch (exception) {
                console.error(exception);
            }

        };

        eventSource.onerror = function (event) {
            console.debug(event);
        };


        // Server Events

        eventSource.addEventListener('OnGetVersionDetailRequest', (event: MessageEvent<string>) => {

            try {

                const request = JSON.parse(event.data);

                const entries = Object.entries(request);
                if (entries.length === 0)
                    return;

                CreateLogEntry(
                    request.timestamp       ?? Date.now(),
                    request.eventTrackingId ?? "",
                    request.remotePartyId,
                    request.from,
                    request.to,
                    "out",
                    "OnGetVersionDetailRequest",
                    `${request.versionId}`,
                    request.remotePartyId ?? "" // ConnectionColorKey
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnGetVersionDetailRequest',
                    event.data,
                    exception
                );
            }

        }, false);

        eventSource.addEventListener('OnGetVersionDetailResponse', (event: MessageEvent<string>) => {

            try {

                const response = JSON.parse(event.data);

                AppendLogEntry(
                    response.timestamp,
                    response.roamingNetwork,
                    response.eventTrackingId,
                    ` ⇒ !`,
                    response.runtime
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnGetVersionDetailResponse',
                    event.data,
                    exception
                );
            }

        }, false);


        eventSource.addEventListener('OnPutLocationRequest', (event: MessageEvent<string>) => {

            try {

                const request = JSON.parse(event.data);

                const entries = Object.entries(request);
                if (entries.length === 0)
                    return;

                CreateLogEntry(
                    request.timestamp ?? Date.now(),
                    request.eventTrackingId ?? "",
                    request.remotePartyId,
                    request.from,
                    request.to,
                    "out",
                    "OnPutLocationRequest",
                    `Location ${request.location.Id}: ${JSON.stringify(request.location)}`,
                    request.remotePartyId ?? "" // ConnectionColorKey
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPutLocationRequest',
                    event.data,
                    exception
                );
            }

        }, false);

        eventSource.addEventListener('OnPutLocationResponse', (event: MessageEvent<string>) => {

            try {

                const response = JSON.parse(event.data);

                AppendLogEntry(
                    response.timestamp,
                    response.roamingNetwork,
                    response.eventTrackingId,
                    ` ⇒ !`,
                    response.runtime
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPutLocationResponse',
                    event.data,
                    exception
                );
            }

        }, false);

        eventSource.addEventListener('OnPatchLocationRequest', (event: MessageEvent<string>) => {

            try {

                const request = JSON.parse(event.data);

                const entries = Object.entries(request);
                if (entries.length === 0)
                    return;

                CreateLogEntry(
                    request.timestamp ?? Date.now(),
                    request.eventTrackingId ?? "",
                    request.remotePartyId,
                    request.from,
                    request.to,
                    "out",
                    "OnPatchLocationRequest",
                    `Location ${request.locationId}: ${JSON.stringify(request.locationPatch)}`,
                    request.remotePartyId ?? "" // ConnectionColorKey
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPatchLocationRequest',
                    event.data,
                    exception
                );
            }

        }, false);

        eventSource.addEventListener('OnPatchLocationResponse', (event: MessageEvent<string>) => {

            try {

                const response = JSON.parse(event.data);

                AppendLogEntry(
                    response.timestamp,
                    response.roamingNetwork,
                    response.eventTrackingId,
                    ` ⇒ !`,
                    response.runtime
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPatchLocationResponse',
                    event.data,
                    exception
                );
            }

        }, false);


        eventSource.addEventListener('OnPutEVSERequest', (event: MessageEvent<string>) => {

            try {

                const request = JSON.parse(event.data);

                const entries = Object.entries(request);
                if (entries.length === 0)
                    return;

                CreateLogEntry(
                    request.timestamp ?? Date.now(),
                    request.eventTrackingId ?? "",
                    request.remotePartyId,
                    request.from,
                    request.to,
                    "out",
                    "OnPutEVSERequest",
                    `EVSE ${request.evse.uid}: ${JSON.stringify(request.evse)}`,
                    request.remotePartyId ?? "" // ConnectionColorKey
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPutEVSERequest',
                    event.data,
                    exception
                );
            }

        }, false);

        eventSource.addEventListener('OnPutEVSEResponse', (event: MessageEvent<string>) => {

            try {

                const response = JSON.parse(event.data);

                AppendLogEntry(
                    response.timestamp,
                    response.roamingNetwork,
                    response.eventTrackingId,
                    ` ⇒ !`,
                    response.runtime
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPutEVSEResponse',
                    event.data,
                    exception
                );
            }

        }, false);

        eventSource.addEventListener('OnPatchEVSERequest', (event: MessageEvent<string>) => {

            try {

                const request = JSON.parse(event.data);

                const entries = Object.entries(request);
                if (entries.length === 0)
                    return;

                CreateLogEntry(
                    request.timestamp       ?? Date.now(),
                    request.eventTrackingId ?? "",
                    request.remotePartyId,
                    request.from,
                    request.to,
                    "out",
                    "OnPatchEVSERequest",
                    `EVSE ${request.evseId}: ${JSON.stringify(request.evsePatch)}`,
                    request.remotePartyId ?? "" // ConnectionColorKey
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPatchEVSERequest',
                    event.data,
                    exception
                );
            }

        }, false);

        eventSource.addEventListener('OnPatchEVSEResponse', (event: MessageEvent<string>) => {

            try {

                const response = JSON.parse(event.data);

                AppendLogEntry(
                    response.timestamp,
                    response.roamingNetwork,
                    response.eventTrackingId,
                    ` ⇒ !`,
                    response.runtime
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPatchEVSEResponse',
                    event.data,
                    exception
                );
            }

        }, false);


        eventSource.addEventListener('OnPutConnectorRequest', (event: MessageEvent<string>) => {

            try {

                const request = JSON.parse(event.data);

                const entries = Object.entries(request);
                if (entries.length === 0)
                    return;

                CreateLogEntry(
                    request.timestamp ?? Date.now(),
                    request.eventTrackingId ?? "",
                    request.remotePartyId,
                    request.from,
                    request.to,
                    "out",
                    "OnPutConnectorRequest",
                    `Connector ${request.connector.id}: ${JSON.stringify(request.connector)}`,
                    request.remotePartyId ?? "" // ConnectionColorKey
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPutConnectorRequest',
                    event.data,
                    exception
                );
            }

        }, false);

        eventSource.addEventListener('OnPutConnectorResponse', (event: MessageEvent<string>) => {

            try {

                const response = JSON.parse(event.data);

                AppendLogEntry(
                    response.timestamp,
                    response.roamingNetwork,
                    response.eventTrackingId,
                    ` ⇒ !`,
                    response.runtime
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPutConnectorResponse',
                    event.data,
                    exception
                );
            }

        }, false);

        eventSource.addEventListener('OnPatchConnectorRequest', (event: MessageEvent<string>) => {

            try {

                const request = JSON.parse(event.data);

                const entries = Object.entries(request);
                if (entries.length === 0)
                    return;

                CreateLogEntry(
                    request.timestamp ?? Date.now(),
                    request.eventTrackingId ?? "",
                    request.remotePartyId,
                    request.from,
                    request.to,
                    "out",
                    "OnPatchConnectorRequest",
                    `Connector ${request.connectorId}: ${JSON.stringify(request.connectorPatch)}`,
                    request.remotePartyId ?? "" // ConnectionColorKey
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPatchConnectorRequest',
                    event.data,
                    exception
                );
            }

        }, false);

        eventSource.addEventListener('OnPatchConnectorResponse', (event: MessageEvent<string>) => {

            try {

                const response = JSON.parse(event.data);

                AppendLogEntry(
                    response.timestamp,
                    response.roamingNetwork,
                    response.eventTrackingId,
                    ` ⇒ !`,
                    response.runtime
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPatchConnectorResponse',
                    event.data,
                    exception
                );
            }

        }, false);


        eventSource.addEventListener('OnPutTariffRequest', (event: MessageEvent<string>) => {

            try {

                const request = JSON.parse(event.data);

                const entries = Object.entries(request);
                if (entries.length === 0)
                    return;

                CreateLogEntry(
                    request.timestamp ?? Date.now(),
                    request.eventTrackingId ?? "",
                    request.remotePartyId,
                    request.from,
                    request.to,
                    "out",
                    "OnPutTariffRequest",
                    `Tariff ${request.tariff.id}: ${JSON.stringify(request.tariff)}`,
                    request.remotePartyId ?? "" // ConnectionColorKey
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPutTariffRequest',
                    event.data,
                    exception
                );
            }

        }, false);

        eventSource.addEventListener('OnPutTariffResponse', (event: MessageEvent<string>) => {

            try {

                const response = JSON.parse(event.data);

                AppendLogEntry(
                    response.timestamp,
                    response.roamingNetwork,
                    response.eventTrackingId,
                    ` ⇒ !`,
                    response.runtime
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnPutTariffResponse',
                    event.data,
                    exception
                );
            }

        }, false);


        eventSource.addEventListener('OnPutSessionRequest', (event: MessageEvent<string>) => {

            try
            {

                const request = JSON.parse(event.data);

                const entries = Object.entries(request);
                if (entries.length === 0)
                    return;

                const session = request.session;
                const token   = session.cdr_token;

                CreateLogEntry(
                    request.timestamp       ?? Date.now(),
                    request.eventTrackingId ?? "",
                    request.remotePartyId,
                    request.from,
                    request.to,
                    "out",
                    "OnPutSessionRequest",
                    `Session: ${session.country_code}-${session.party_id} ${session.id}: <b>${session.status}</b>, ${session.kwh} kWh, ${session.total_cost != null ? session.total_cost + " " + session.currency : ""}<br />` +
                    `Location: ${session.location_id}, evse: ${session.evse_uid}, connector: ${session.connector_id}<br />` +
                    `Token: ${token.country_code}-${token.party_id}-${token.uid} (${token.type}), contract: ${token.contract_id}, auth_ref: ${session.authorization_reference}<br />`,
                    request.remotePartyId ?? "" // ConnectionColorKey
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
                    ` ⇒ !`,
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
                    request.timestamp       ?? Date.now(),
                    request.eventTrackingId ?? "",
                    request.remotePartyId,
                    request.from,
                    request.to,
                    "out",
                    "OnPatchSessionRequest",
                    `${request.sessionId}`,
                    request.remotePartyId ?? "" // ConnectionColorKey
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
                    ` ⇒ !`,
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

                const request   = JSON.parse(event.data);

                const entries   = Object.entries(request);
                if (entries.length === 0)
                    return;

                const cdr       = request.cdr;
                const token     = cdr.cdr_token;
                const location  = cdr.cdr_location;

                CreateLogEntry(
                    request.timestamp       ?? Date.now(),
                    request.eventTrackingId ?? "",
                    request.remotePartyId,
                    request.from,
                    request.to,
                    "out",
                    "OnPostCDRRequest",
                    `Id: ${cdr.country_code}-${cdr.party_id} ${cdr.id} ${cdr.session_id != null ? ` for session: ${cdr.session_id}` : ""}<br />` +
                    `Start: ${cdr.start_date_time}, stop: ${cdr.end_date_time}<br />` +
                    `${cdr.total_time} hours, ${cdr.total_energy} kWh, ${cdr.total_cost.excl_vat} ${cdr.currency}<br />` +
                    `Location: ${location.id}, evse: ${location.evse_uid} (${location.evse_id}), connector: ${location.connector_id} (${location.connector_standard}/${location.connector_format}/${location.connector_power_type})<br />` +
                    `Token: ${token.country_code}-${token.party_id}-${token.uid} (${token.type}), contract: ${token.contract_id}, auth: ${cdr.auth_method} / ${cdr.authorization_reference ?? "-"}<br />`,
                    request.remotePartyId ?? "" // ConnectionColorKey
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
                    ` ⇒ ${response.cdrLocation}`,
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
                    request.timestamp       ?? Date.now(),
                    request.eventTrackingId ?? "",
                    request.remotePartyId,
                    request.from,
                    request.to,
                    "out",
                    "OnPostTokenRequest",
                    `${request.tokenId} (${request.requestedTokenType})`,
                    request.remotePartyId ?? "" // ConnectionColorKey
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
                    ` ⇒ ${response.authorizationInfo.allowed} (${response.authorizationInfoauthorization_reference ?? "-"})`,
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



        // Client Events

        eventSource.addEventListener('OnStartSessionRequest', (event: MessageEvent<string>) => {

            try
            {

                const request   = JSON.parse(event.data);

                const entries   = Object.entries(request);
                if (entries.length === 0)
                    return;

                const token = request.token;

                CreateLogEntry(
                    request.timestamp       ?? Date.now(),
                    request.eventTrackingId ?? "",
                    request.remotePartyId,
                    request.from,
                    request.to,
                    "in",
                    "OnStartSessionRequest",
                    `Token: ${token.country_code}-${token.party_id}-${token.uid} (${token.type}), contract: ${token.contract_id}, auth: ${request.authorizationReference ?? "-"}<br />` +
                    `Location: ${request.locationId}, evse: ${request.evseUId ?? "-"}, connector: ${request.connectorId ?? "-"}`,
                    request.remotePartyId ?? "" // ConnectionColorKey
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnStartSessionRequest',
                    event.data,
                    exception
                );
            }

        }, false);

        eventSource.addEventListener('OnStartSessionResponse', (event: MessageEvent<string>) => {

            try
            {

                const response         = JSON.parse(event.data);

                const ocpiResponse     = response.response;
                const commandResponse  = ocpiResponse.data;
                const commandResult    = commandResponse?.result ?? `Status code: ${ocpiResponse.status_code}${ocpiResponse.status_message != null ? `<br />"Status message: ${ocpiResponse.status_message}"` : ""}`;

                AppendLogEntry(
                    response.timestamp,
                    response.roamingNetwork,
                    response.eventTrackingId,
                    ` ⇒ ${commandResult}${multiLanguageTextToHTML(commandResponse?.message ?? [])}` +
                    `${ocpiResponse.additionalInformation != null ? `<br />"Additional information: ${ocpiResponse.additionalInformation}"` : ""}`,
                    response.runtime
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnStartSessionResponse',
                    event.data,
                    exception
                );
            }

        }, false);


        eventSource.addEventListener('OnStopSessionRequest', (event: MessageEvent<string>) => {

            try
            {

                const request   = JSON.parse(event.data);

                const entries   = Object.entries(request);
                if (entries.length === 0)
                    return;

                CreateLogEntry(
                    request.timestamp       ?? Date.now(),
                    request.eventTrackingId ?? "",
                    request.remotePartyId,
                    request.from,
                    request.to,
                    "in",
                    "OnStopSessionRequest",
                    `Stop session: ${request.sessionId}`,
                    request.remotePartyId ?? "" // ConnectionColorKey
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnStopSessionRequest',
                    event.data,
                    exception
                );
            }

        }, false);

        eventSource.addEventListener('OnStopSessionResponse', (event: MessageEvent<string>) => {

            try
            {

                const response         = JSON.parse(event.data);

                const ocpiResponse     = response.response;
                const commandResponse  = ocpiResponse.data;
                const commandResult    = commandResponse?.result ?? `Status code: ${ocpiResponse.status_code}${ocpiResponse.status_message != null ? `<br />"Status message: ${ocpiResponse.status_message}"` : ""}`;

                AppendLogEntry(
                    response.timestamp,
                    response.roamingNetwork,
                    response.eventTrackingId,
                    ` ⇒ ${commandResult}${multiLanguageTextToHTML(commandResponse?.message ?? [])}` +
                    `${ocpiResponse.additionalInformation != null ? `<br />"Additional information: ${ocpiResponse.additionalInformation}"` : ""}`,
                    response.runtime
                );

            }
            catch (exception) {
                ShowHTTPSSEError(
                    'OnStopSessionResponse',
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
