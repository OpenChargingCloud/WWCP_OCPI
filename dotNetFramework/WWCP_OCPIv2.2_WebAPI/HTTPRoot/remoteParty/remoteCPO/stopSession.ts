///<reference path="../../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

function StartStopSession(versionId: string) {

    const pathElements          = window.location.pathname.split("/");
    const remotePartyId         = pathElements[pathElements.length - 2];

    const stopSessionInfos      = document.getElementById("stopSessionInfos")           as HTMLDivElement;
    const remotePartyIdDiv      = stopSessionInfos.  querySelector("#id")               as HTMLDivElement;
    remotePartyIdDiv.innerText  = remotePartyId;

    const stopSessionDiv        = stopSessionInfos.  querySelector("#stopSession")      as HTMLDivElement;

    const remotePartyDataDiv    = stopSessionDiv.    querySelector("#data")             as HTMLDivElement;
    const sessionId             = remotePartyDataDiv.querySelector("#sessionId")        as HTMLInputElement;

    const lowerButtonsDiv       = stopSessionDiv.    querySelector(".lowerButtons")     as HTMLDivElement;
    const stopSessionButton     = lowerButtonsDiv.   querySelector("#stopSession")      as HTMLButtonElement;

    stopSessionButton.onclick = () => {

        stopSessionButton.disabled = true;

        const _sessionId  = sessionId.value?.trim();

        if (_sessionId !== "")
        {

            HTTP("StopSession",
                 "../" + remotePartyId,
                 {
                     "sessionId": _sessionId
                 },

                 (status, response) => {

                     try
                     {

                         const remoteParty = ParseJSON_LD<IRemoteParty>(response);

                         if (remoteParty != null)
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

                     stopSessionButton.disabled = false;

                 },

                 (status, statusText, response) => {

                     stopSessionButton.disabled = false;

                 }

                );

        }

    };


    HTTPGet("stopSession",

            (status, response) => {

                try
                {

                    const remoteParty = ParseJSON_LD<IRemoteParty>(response);

                    if (remoteParty != null)
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
