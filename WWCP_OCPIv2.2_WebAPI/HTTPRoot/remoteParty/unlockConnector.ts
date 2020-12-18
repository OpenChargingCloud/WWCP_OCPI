///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

function StartUnlockConnector(versionId: string) {

    const pathElements             = window.location.pathname.split("/");
    const remotePartyId            = pathElements[pathElements.length - 2];

    const unlockConnectorInfos     = document.getElementById("unlockConnectorInfos")                 as HTMLDivElement;
    const remotePartyIdDiv         = unlockConnectorInfos.querySelector("#id")                       as HTMLDivElement;
    remotePartyIdDiv.innerText     = remotePartyId;

    const unlockConnectorDiv       = unlockConnectorInfos.querySelector("#unlockConnector")          as HTMLDivElement;

    const remotePartyDataDiv       = unlockConnectorDiv. querySelector("#data")                      as HTMLDivElement;
    const locationId               = remotePartyDataDiv. querySelector("#locationId")                as HTMLInputElement;
    const EVSEUId                  = remotePartyDataDiv. querySelector("#EVSEUId")                   as HTMLInputElement;
    const connectorId              = remotePartyDataDiv. querySelector("#connectorId")               as HTMLInputElement;

    const lowerButtonsDiv          = unlockConnectorDiv. querySelector("#lowerButtons")              as HTMLDivElement;
    const unlockConnectorButton    = lowerButtonsDiv.    querySelector("#unlockConnector")           as HTMLButtonElement;

    unlockConnectorButton.onclick = () => {

        unlockConnectorButton.disabled = true;

        const _locationId   = locationId. value?.trim();
        const _EVSEUId      = EVSEUId.    value?.trim();
        const _connectorId  = connectorId.value?.trim();

        if (_locationId  !== "" &&
            _EVSEUId     !== "" &&
            _connectorId !== "")
        {

            HTTP("UnlockConnector",
                 "../" + remotePartyId,
                 {
                     "locationId":  _locationId,
                     "EVSEUId":     _EVSEUId,
                     "connectorId": _connectorId
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

                 },

                 (status, statusText, response) => {
                 }

                );

            //document.location.href = document.location.href + "/unlockConnector";

        }


    };


    HTTPGet("unlockConnector",

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
