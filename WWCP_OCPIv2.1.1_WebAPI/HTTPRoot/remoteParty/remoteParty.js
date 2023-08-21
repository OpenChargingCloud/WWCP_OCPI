///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
function StartRemoteParty(versionId) {
    const pathElements = window.location.pathname.split("/");
    const remotePartyId = pathElements[pathElements.length - 1];
    const remotePartyInfosDiv = document.getElementById("remotePartyInfos");
    const remotePartyIdDiv = remotePartyInfosDiv.querySelector("#id");
    remotePartyIdDiv.innerText = remotePartyId;
    const remotePartyDiv = remotePartyInfosDiv.querySelector("#remoteParty");
    const remotePartyDataDiv = remotePartyDiv.querySelector("#data");
    const countryCode = remotePartyDataDiv.querySelector("#countryCode");
    const partyId = remotePartyDataDiv.querySelector("#partyId");
    const role = remotePartyDataDiv.querySelector("#role");
    const businessDetailsName = remotePartyDataDiv.querySelector("#businessDetailsName");
    const businessDetailsWebsite = remotePartyDataDiv.querySelector("#businessDetailsWebsite");
    const businessDetailsLogo = remotePartyDataDiv.querySelector("#businessDetailsLogo");
    const remoteAccessInfosDiv = remotePartyDiv.querySelector("#remoteAccessInfosDiv");
    const remoteAccessInfos = remoteAccessInfosDiv.querySelector("#remoteAccessInfos");
    const accessInfosDiv = remotePartyDiv.querySelector("#accessInfosDiv");
    const accessInfos = accessInfosDiv.querySelector("#accessInfos");
    const lowerButtonsDiv = remotePartyDiv.querySelector(".lowerButtons");
    const registerButton = lowerButtonsDiv.querySelector("#register");
    const registrationUpdateButton = lowerButtonsDiv.querySelector("#registrationUpdate");
    const reserveNowButton = lowerButtonsDiv.querySelector("#reserveNow");
    const cancelReservationButton = lowerButtonsDiv.querySelector("#cancelReservation");
    const startSessionButton = lowerButtonsDiv.querySelector("#startSession");
    const stopSessionButton = lowerButtonsDiv.querySelector("#stopSession");
    const unlockConnectorButton = lowerButtonsDiv.querySelector("#unlockConnector");
    registerButton.onclick = () => { document.location.href = document.location.href + "/register"; };
    registrationUpdateButton.onclick = () => { document.location.href = document.location.href + "/registrationUpdate"; };
    // Remote CPOs
    reserveNowButton.onclick = () => { document.location.href = document.location.href + "/reserveNow"; };
    cancelReservationButton.onclick = () => { document.location.href = document.location.href + "/cancelReservation"; };
    startSessionButton.onclick = () => { document.location.href = document.location.href + "/startSession"; };
    stopSessionButton.onclick = () => { document.location.href = document.location.href + "/stopSession"; };
    unlockConnectorButton.onclick = () => { document.location.href = document.location.href + "/unlockConnector"; };
    HTTPGet("../remoteParties/" + remotePartyId, (status, response) => {
        var _a, _b, _c;
        try {
            const remoteParty = ParseJSON_LD(response);
            if (remoteParty != null) {
                countryCode.value = remoteParty.countryCode;
                partyId.value = remoteParty.partyId;
                role.value = remoteParty.role;
                if (remoteParty.businessDetails !== undefined) {
                    businessDetailsName.value = (_a = remoteParty.businessDetails.name) !== null && _a !== void 0 ? _a : "";
                    businessDetailsWebsite.value = (_b = remoteParty.businessDetails.website) !== null && _b !== void 0 ? _b : "";
                    businessDetailsLogo.value = (_c = remoteParty.businessDetails.logo) !== null && _c !== void 0 ? _c : "";
                }
                if (remoteParty.remoteAccessInfos !== undefined &&
                    remoteParty.remoteAccessInfos.length > 0) {
                    for (const remoteAccessInfo of remoteParty.remoteAccessInfos) {
                        const remoteAccessInfoDiv = remoteAccessInfos.appendChild(document.createElement('div'));
                        remoteAccessInfoDiv.className = "remoteAccessInfo";
                        const remoteAccessURL = remoteAccessInfoDiv.appendChild(document.createElement('div'));
                        remoteAccessURL.className = "row";
                        const remoteAccessURLKey = remoteAccessURL.appendChild(document.createElement('div'));
                        remoteAccessURLKey.className = "subkey";
                        remoteAccessURLKey.innerHTML = "Versions URL";
                        const remoteAccessURLValue = remoteAccessURL.appendChild(document.createElement('div'));
                        remoteAccessURLValue.className = "value";
                        const remoteAccessURLInput = remoteAccessURLValue.appendChild(document.createElement('input'));
                        remoteAccessURLInput.value = remoteAccessInfo.versionsURL;
                        const remoteAccessToken = remoteAccessInfoDiv.appendChild(document.createElement('div'));
                        remoteAccessToken.className = "row";
                        const remoteAccessTokenKey = remoteAccessToken.appendChild(document.createElement('div'));
                        remoteAccessTokenKey.className = "subkey";
                        remoteAccessTokenKey.innerHTML = "Access Token";
                        const remoteAccessTokenValue = remoteAccessToken.appendChild(document.createElement('div'));
                        remoteAccessTokenValue.className = "value";
                        const remoteAccessTokenInput = remoteAccessTokenValue.appendChild(document.createElement('input'));
                        remoteAccessTokenInput.value = remoteAccessInfo.accessToken;
                        const remoteAccessStatus = remoteAccessInfoDiv.appendChild(document.createElement('div'));
                        remoteAccessStatus.className = "row";
                        const remoteAccessStatusKey = remoteAccessStatus.appendChild(document.createElement('div'));
                        remoteAccessStatusKey.className = "subkey";
                        remoteAccessStatusKey.innerHTML = "Status";
                        const remoteAccessStatusValue = remoteAccessStatus.appendChild(document.createElement('div'));
                        remoteAccessStatusValue.className = "value";
                        const remoteAccessStatusInput = remoteAccessStatusValue.appendChild(document.createElement('input'));
                        remoteAccessStatusInput.value = remoteAccessInfo.status;
                    }
                }
                if (remoteParty.accessInfos !== undefined &&
                    remoteParty.accessInfos.length > 0) {
                    for (const accessInfo of remoteParty.accessInfos) {
                        const accessInfoDiv = accessInfos.appendChild(document.createElement('div'));
                        accessInfoDiv.className = "accessInfo";
                        const accessToken = accessInfoDiv.appendChild(document.createElement('div'));
                        accessToken.className = "row";
                        const accessTokenKey = accessToken.appendChild(document.createElement('div'));
                        accessTokenKey.className = "subkey";
                        accessTokenKey.innerHTML = "Access Token";
                        const accessTokenValue = accessToken.appendChild(document.createElement('div'));
                        accessTokenValue.className = "value";
                        const accessTokenInput = accessTokenValue.appendChild(document.createElement('input'));
                        accessTokenInput.value = accessInfo.token;
                        const accessStatus = accessInfoDiv.appendChild(document.createElement('div'));
                        accessStatus.className = "row";
                        const accessStatusKey = accessStatus.appendChild(document.createElement('div'));
                        accessStatusKey.className = "subkey";
                        accessStatusKey.innerHTML = "Status";
                        const accessStatusValue = accessStatus.appendChild(document.createElement('div'));
                        accessStatusValue.className = "value";
                        const accessStatusInput = accessStatusValue.appendChild(document.createElement('input'));
                        accessStatusInput.value = accessInfo.status;
                    }
                }
            }
        }
        catch (exception) {
        }
    }, (status, statusText, response) => {
    });
    //var refresh = setTimeout(StartDashboard, 30000);
}
//# sourceMappingURL=remoteParty.js.map