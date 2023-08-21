///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
function StartRemoteParties() {
    const remotePartiesInfosDiv = document.getElementById("remotePartiesInfos");
    const remotePartiesDiv = remotePartiesInfosDiv.querySelector("#remoteParties");
    HTTPGet("remoteParties?withMetadata", (status, response) => {
        var _a, _b, _c;
        try {
            const results = ParseJSON_LD(response);
            if ((results === null || results === void 0 ? void 0 : results.searchResults) != undefined &&
                (results === null || results === void 0 ? void 0 : results.searchResults) != null &&
                Array.isArray(results.searchResults) &&
                results.searchResults.length > 0) {
                for (const remoteParty of results.searchResults) {
                    const remotePartyDiv = remotePartiesDiv.appendChild(document.createElement('a'));
                    remotePartyDiv.className = "remoteParty";
                    remotePartyDiv.href = "remoteParties/" + remoteParty["@id"];
                    const partyHeadlineDiv = remotePartyDiv.appendChild(document.createElement('div'));
                    partyHeadlineDiv.className = "partyHeadline";
                    partyHeadlineDiv.innerHTML = remoteParty.countryCode + "-" + remoteParty.partyId + " (" + remoteParty.role + ") " + ((_a = remoteParty === null || remoteParty === void 0 ? void 0 : remoteParty.businessDetails) === null || _a === void 0 ? void 0 : _a.name);
                    const remoteTokensDiv = remotePartyDiv.appendChild(document.createElement('div'));
                    remoteTokensDiv.className = "remoteTokens";
                    remoteTokensDiv.innerHTML = ((_b = remoteParty.remoteAccessInfos) === null || _b === void 0 ? void 0 : _b.length) > 0
                        ? (remoteParty.remoteAccessInfos.map(remoteAccessInfo => remoteAccessInfo.versionsURL + "<br />" +
                            "<i class=\"far fa-arrow-alt-circle-right\"></i>" + EncodeToken(remoteAccessInfo.accessToken) + "<br />").
                            join())
                        : "";
                    const accessTokensDiv = remotePartyDiv.appendChild(document.createElement('div'));
                    accessTokensDiv.className = "accessTokens";
                    accessTokensDiv.innerHTML = ((_c = remoteParty.accessInfos) === null || _c === void 0 ? void 0 : _c.length) > 0
                        ? (remoteParty.accessInfos.map(accessInfo => "<i class=\"far fa-arrow-alt-circle-left\"></i>" + EncodeToken(accessInfo.token) + "<br />").
                            join())
                        : "";
                }
            }
        }
        catch (exception) {
        }
    }, (status, statusText, response) => {
    });
    //var refresh = setTimeout(StartDashboard, 30000);
}
//# sourceMappingURL=remoteParties.js.map