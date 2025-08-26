
function StartCancelReservation(versionId: string) {

    const pathElements             = window.location.pathname.split("/");
    const remotePartyId            = pathElements[pathElements.length - 2];

    const cancelReservationInfos   = document.getElementById("cancelReservationInfos")              as HTMLDivElement;
    const remotePartyIdDiv         = cancelReservationInfos.querySelector("#id")                    as HTMLDivElement;
    remotePartyIdDiv.innerText     = remotePartyId;

    const cancelReservationDiv     = cancelReservationInfos.querySelector("#cancelReservation")     as HTMLDivElement;

    const remotePartyDataDiv       = cancelReservationDiv.  querySelector("#data")                  as HTMLDivElement;
    const reservationId            = remotePartyDataDiv.    querySelector("#reservationId")         as HTMLInputElement;

    const lowerButtonsDiv          = cancelReservationDiv.  querySelector(".lowerButtons")          as HTMLDivElement;
    const cancelReservationButton  = lowerButtonsDiv.       querySelector("#cancelReservation")     as HTMLButtonElement;

    cancelReservationButton.onclick = () => {

        cancelReservationButton.disabled = true;

        const _reservationId  = reservationId.value?.trim();

        if (_reservationId !== "")
        {

            HTTP("CancelReservation",
                 "../" + remotePartyId,
                 {
                     "reservationId": _reservationId
                 },

                 (status, response) => {

                     try
                     {

                         const remoteParty = ParseJSON_LD<IRemoteParty>(response);

                         if (remoteParty !== null)
                         {

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

                 },

                 (status, statusText, response) => {
                 }

                );

            //document.location.href = document.location.href + "/unlockConnector";

        }


    };


    HTTPGet("cancelReservation",

            (status, response) => {

                try
                {

                    const remoteParty = ParseJSON_LD<IRemoteParty>(response);

                    if (remoteParty !== null)
                    {

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

            },

            (status, statusText, response) => {
            }

    );

    //var refresh = setTimeout(StartDashboard, 30000);

}
