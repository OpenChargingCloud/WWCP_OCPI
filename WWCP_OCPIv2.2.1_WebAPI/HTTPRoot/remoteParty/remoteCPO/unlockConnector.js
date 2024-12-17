function StartUnlockConnector(versionId) {
    function ToogleUnlockConnectorButton() {
        var _a, _b, _c;
        const _locationId = (_a = locationId.value) === null || _a === void 0 ? void 0 : _a.trim();
        const _EVSEUId = (_b = EVSEUId.value) === null || _b === void 0 ? void 0 : _b.trim();
        const _connectorId = (_c = connectorId.value) === null || _c === void 0 ? void 0 : _c.trim();
        unlockConnectorButton.disabled = _locationId.length < 2 || _EVSEUId.length < 2 || _connectorId.length < 1;
    }
    const pathElements = window.location.pathname.split("/");
    const remotePartyId = pathElements[pathElements.length - 2];
    const unlockConnectorInfos = document.getElementById("unlockConnectorInfos");
    const remotePartyIdDiv = unlockConnectorInfos.querySelector("#id");
    remotePartyIdDiv.innerText = remotePartyId;
    const unlockConnectorDiv = unlockConnectorInfos.querySelector("#unlockConnector");
    const remotePartyDataDiv = unlockConnectorDiv.querySelector("#data");
    const locationId = remotePartyDataDiv.querySelector("#locationId");
    const EVSEUId = remotePartyDataDiv.querySelector("#EVSEUId");
    const connectorId = remotePartyDataDiv.querySelector("#connectorId");
    const responseDiv = unlockConnectorDiv.querySelector("#response");
    const lowerButtonsDiv = unlockConnectorDiv.querySelector(".lowerButtons");
    const unlockConnectorButton = lowerButtonsDiv.querySelector("#unlockConnector");
    unlockConnectorButton.onclick = () => {
        var _a, _b, _c;
        unlockConnectorButton.disabled = true;
        responseDiv.innerHTML = "";
        const _locationId = (_a = locationId.value) === null || _a === void 0 ? void 0 : _a.trim();
        const _EVSEUId = (_b = EVSEUId.value) === null || _b === void 0 ? void 0 : _b.trim();
        const _connectorId = (_c = connectorId.value) === null || _c === void 0 ? void 0 : _c.trim();
        if (_locationId !== "" &&
            _EVSEUId !== "" &&
            _connectorId !== "") {
            HTTP("UnlockConnector", "../" + remotePartyId, {
                "locationId": _locationId,
                "EVSEUId": _EVSEUId,
                "connectorId": _connectorId
            }, (status, response) => {
                try {
                    const remoteParty = ParseJSON_LD(response);
                    if (remoteParty != null) {
                        responseDiv.innerHTML = remoteParty.toString();
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
                unlockConnectorButton.disabled = false;
            }, (status, statusText, response) => {
                unlockConnectorButton.disabled = false;
            });
        }
    };
    locationId.oninput = () => { ToogleUnlockConnectorButton(); };
    EVSEUId.oninput = () => { ToogleUnlockConnectorButton(); };
    connectorId.oninput = () => { ToogleUnlockConnectorButton(); };
    HTTPGet("unlockConnector", (status, response) => {
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
//# sourceMappingURL=unlockConnector.js.map