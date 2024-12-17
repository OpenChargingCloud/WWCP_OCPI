function StartCancelReservation(versionId) {
    const pathElements = window.location.pathname.split("/");
    const remotePartyId = pathElements[pathElements.length - 2];
    const cancelReservationInfos = document.getElementById("cancelReservationInfos");
    const remotePartyIdDiv = cancelReservationInfos.querySelector("#id");
    remotePartyIdDiv.innerText = remotePartyId;
    const cancelReservationDiv = cancelReservationInfos.querySelector("#cancelReservation");
    const remotePartyDataDiv = cancelReservationDiv.querySelector("#data");
    const reservationId = remotePartyDataDiv.querySelector("#reservationId");
    const lowerButtonsDiv = cancelReservationDiv.querySelector(".lowerButtons");
    const cancelReservationButton = lowerButtonsDiv.querySelector("#cancelReservation");
    cancelReservationButton.onclick = () => {
        var _a;
        cancelReservationButton.disabled = true;
        const _reservationId = (_a = reservationId.value) === null || _a === void 0 ? void 0 : _a.trim();
        if (_reservationId !== "") {
            HTTP("CancelReservation", "../" + remotePartyId, {
                "reservationId": _reservationId
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
            }, (status, statusText, response) => {
            });
            //document.location.href = document.location.href + "/unlockConnector";
        }
    };
    HTTPGet("cancelReservation", (status, response) => {
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
//# sourceMappingURL=cancelReservation.js.map