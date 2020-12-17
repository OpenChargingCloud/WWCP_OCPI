///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
function StartUnlockConnector(versionId) {
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
    const lowerButtonsDiv = unlockConnectorDiv.querySelector("#lowerButtons");
    const unlockConnectorButton = lowerButtonsDiv.querySelector("#unlockConnector");
    unlockConnectorButton.onclick = () => {
        unlockConnectorButton.disabled = true;
        var commandJSON = {
            "locationId": locationId.value.trim(),
            "EVSEUId": EVSEUId.value.trim(),
            "connectorId": connectorId.value.trim()
        };
        //document.location.href = document.location.href + "/unlockConnector";
    };
    HTTPGet("../remoteParties/" + remotePartyId + "/unlockConnector", (status, response) => {
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