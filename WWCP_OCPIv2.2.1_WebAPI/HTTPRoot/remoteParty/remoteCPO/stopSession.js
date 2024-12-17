function StartStopSession(versionId) {
    const pathElements = window.location.pathname.split("/");
    const remotePartyId = pathElements[pathElements.length - 2];
    const stopSessionInfos = document.getElementById("stopSessionInfos");
    const remotePartyIdDiv = stopSessionInfos.querySelector("#id");
    remotePartyIdDiv.innerText = remotePartyId;
    const stopSessionDiv = stopSessionInfos.querySelector("#stopSession");
    const remotePartyDataDiv = stopSessionDiv.querySelector("#data");
    const sessionId = remotePartyDataDiv.querySelector("#sessionId");
    const lowerButtonsDiv = stopSessionDiv.querySelector(".lowerButtons");
    const stopSessionButton = lowerButtonsDiv.querySelector("#stopSession");
    stopSessionButton.onclick = () => {
        var _a;
        stopSessionButton.disabled = true;
        const _sessionId = (_a = sessionId.value) === null || _a === void 0 ? void 0 : _a.trim();
        if (_sessionId !== "") {
            HTTP("StopSession", "../" + remotePartyId, {
                "sessionId": _sessionId
            }, (status, response) => {
                try {
                    const remoteParty = ParseJSON_LD(response);
                    if (remoteParty != null) {
                        //countryCode.value  = remoteParty.countryCode;
                        //partyId.value      = remoteParty.partyId;
                        //role.value         = remoteParty.role;
                        //if (remoteParty.businessDetails !== undefined) {
                        //    businessDetailsName.value     = remoteParty.businessDetails.name    ?? "";
                        //    businessDetailsWebsite.value  = remoteParty.businessDetails.website ?? "";
                        //    businessDetailsLogo.value     = remoteParty.businessDetails.logo    ?? "";
                        //}
                        //const versionDetails = OCPIResponse.data as IVersionDetails;
                        //versionIdDiv.innerHTML = "Version " + versionDetails.version;
                        //for (const endpoint of versionDetails.endpoints) {
                        //    const endpointDiv      = endpointsDiv.appendChild(document.createElement('a')) as HTMLAnchorElement;
                        //    endpointDiv.className  = "endpoint";
                        //    endpointDiv.href       = endpoint.url;
                        //    endpointDiv.innerHTML  = endpoint.identifier + "/" + endpoint.role + "<br /><span class=\"url\">" + endpoint.url + "</span>";
                        //}
                    }
                }
                catch (exception) {
                }
                stopSessionButton.disabled = false;
            }, (status, statusText, response) => {
                stopSessionButton.disabled = false;
            });
        }
    };
    HTTPGet("stopSession", (status, response) => {
        try {
            const remoteParty = ParseJSON_LD(response);
            if (remoteParty != null) {
                //countryCode.value  = remoteParty.countryCode;
                //partyId.value      = remoteParty.partyId;
                //role.value         = remoteParty.role;
                //if (remoteParty.businessDetails !== undefined) {
                //    businessDetailsName.value     = remoteParty.businessDetails.name    ?? "";
                //    businessDetailsWebsite.value  = remoteParty.businessDetails.website ?? "";
                //    businessDetailsLogo.value     = remoteParty.businessDetails.logo    ?? "";
                //}
                //const versionDetails = OCPIResponse.data as IVersionDetails;
                //versionIdDiv.innerHTML = "Version " + versionDetails.version;
                //for (const endpoint of versionDetails.endpoints) {
                //    const endpointDiv      = endpointsDiv.appendChild(document.createElement('a')) as HTMLAnchorElement;
                //    endpointDiv.className  = "endpoint";
                //    endpointDiv.href       = endpoint.url;
                //    endpointDiv.innerHTML  = endpoint.identifier + "/" + endpoint.role + "<br /><span class=\"url\">" + endpoint.url + "</span>";
                //}
            }
        }
        catch (exception) {
        }
    }, (status, statusText, response) => {
    });
    //var refresh = setTimeout(StartDashboard, 30000);
}
//# sourceMappingURL=stopSession.js.map