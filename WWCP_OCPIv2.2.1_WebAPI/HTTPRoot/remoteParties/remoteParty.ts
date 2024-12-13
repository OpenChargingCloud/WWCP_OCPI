///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

function StartRemoteParty(versionId: string) {

    const pathElements                = window.location.pathname.split("/");
    const remotePartyId               = pathElements[pathElements.length - 1];

    const remotePartyInfosDiv         = document.getElementById("remotePartyInfos")                      as HTMLDivElement;
    const remotePartyIdDiv            = remotePartyInfosDiv. querySelector("#id")                        as HTMLDivElement;
    remotePartyIdDiv.innerText        = remotePartyId;

    const remotePartyDiv              = remotePartyInfosDiv. querySelector("#remoteParty")               as HTMLDivElement;

    const remotePartyDataDiv          = remotePartyDiv.      querySelector("#data")                      as HTMLDivElement;
    const countryCode                 = remotePartyDataDiv.  querySelector("#countryCode")               as HTMLInputElement;
    const partyId                     = remotePartyDataDiv.  querySelector("#partyId")                   as HTMLInputElement;
    const role                        = remotePartyDataDiv.  querySelector("#role")                      as HTMLInputElement;
    const businessDetailsName         = remotePartyDataDiv.  querySelector("#businessDetailsName")       as HTMLInputElement;
    const businessDetailsWebsite      = remotePartyDataDiv.  querySelector("#businessDetailsWebsite")    as HTMLInputElement;
    const businessDetailsLogo         = remotePartyDataDiv.  querySelector("#businessDetailsLogo")       as HTMLInputElement;

    const remoteAccessInfosDiv        = remotePartyDiv.      querySelector("#remoteAccessInfosDiv")      as HTMLDivElement;
    const remoteAccessInfos           = remoteAccessInfosDiv.querySelector("#remoteAccessInfos")         as HTMLDivElement;
    const accessInfosDiv              = remotePartyDiv.      querySelector("#accessInfosDiv")            as HTMLDivElement;
    const accessInfos                 = accessInfosDiv.      querySelector("#accessInfos")               as HTMLDivElement;

    const lowerButtonsDiv             = remotePartyDiv.      querySelector(".lowerButtons")              as HTMLDivElement;
    const registerButton              = lowerButtonsDiv.     querySelector("#register")                  as HTMLButtonElement;
    const registrationUpdateButton    = lowerButtonsDiv.     querySelector("#registrationUpdate")        as HTMLButtonElement;

    const reserveNowButton            = lowerButtonsDiv.     querySelector("#reserveNow")                as HTMLButtonElement;
    const cancelReservationButton     = lowerButtonsDiv.     querySelector("#cancelReservation")         as HTMLButtonElement;
    const startSessionButton          = lowerButtonsDiv.     querySelector("#startSession")              as HTMLButtonElement;
    const stopSessionButton           = lowerButtonsDiv.     querySelector("#stopSession")               as HTMLButtonElement;
    const unlockConnectorButton       = lowerButtonsDiv.     querySelector("#unlockConnector")           as HTMLButtonElement;

    registerButton.onclick            = () => { document.location.href = document.location.href + "/register"; };
    registrationUpdateButton.onclick  = () => { document.location.href = document.location.href + "/registrationUpdate"; };

    // Remote CPOs
    reserveNowButton.onclick          = () => { document.location.href = document.location.href + "/reserveNow"; };
    cancelReservationButton.onclick   = () => { document.location.href = document.location.href + "/cancelReservation"; };
    startSessionButton.onclick        = () => { document.location.href = document.location.href + "/startSession"; };
    stopSessionButton.onclick         = () => { document.location.href = document.location.href + "/stopSession"; };
    unlockConnectorButton.onclick     = () => { document.location.href = document.location.href + "/unlockConnector"; };

    HTTPGet("../remoteParties/" + remotePartyId,

        (status, response) => {

            try
            {

                const remoteParty = ParseJSON_LD<IRemoteParty>(response);

                if (remoteParty != null)
                {

                    countryCode.value  = remoteParty.countryCode;
                    partyId.value      = remoteParty.partyId;
                    role.value         = remoteParty.role;

                    if (remoteParty.businessDetails !== undefined) {
                        businessDetailsName.value     = remoteParty.businessDetails.name    ?? "";
                        businessDetailsWebsite.value  = remoteParty.businessDetails.website ?? "";
                        businessDetailsLogo.value     = remoteParty.businessDetails.logo    ?? "";
                    }

                    if (remoteParty.remoteAccessInfos !== undefined &&
                        remoteParty.remoteAccessInfos.length > 0) {

                        for (const remoteAccessInfo of remoteParty.remoteAccessInfos) {

                            const remoteAccessInfoDiv          = remoteAccessInfos.appendChild(document.createElement('div')) as HTMLDivElement;
                            remoteAccessInfoDiv.className      = "remoteAccessInfo";


                            const remoteAccessURL              = remoteAccessInfoDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            remoteAccessURL.className          = "row";

                            const remoteAccessURLKey           = remoteAccessURL.appendChild(document.createElement('div')) as HTMLDivElement;
                            remoteAccessURLKey.className       = "subkey";
                            remoteAccessURLKey.innerHTML       = "Versions URL";

                            const remoteAccessURLValue         = remoteAccessURL.appendChild(document.createElement('div')) as HTMLDivElement;
                            remoteAccessURLValue.className     = "value";

                            const remoteAccessURLInput         = remoteAccessURLValue.appendChild(document.createElement('input')) as HTMLInputElement;
                            remoteAccessURLInput.value         = remoteAccessInfo.versionsURL;


                            const remoteAccessToken            = remoteAccessInfoDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            remoteAccessToken.className        = "row";

                            const remoteAccessTokenKey         = remoteAccessToken.appendChild(document.createElement('div')) as HTMLDivElement;
                            remoteAccessTokenKey.className     = "subkey";
                            remoteAccessTokenKey.innerHTML     = "Access Token";

                            const remoteAccessTokenValue       = remoteAccessToken.appendChild(document.createElement('div')) as HTMLDivElement;
                            remoteAccessTokenValue.className   = "value";

                            const remoteAccessTokenInput       = remoteAccessTokenValue.appendChild(document.createElement('input')) as HTMLInputElement;
                            remoteAccessTokenInput.value       = remoteAccessInfo.accessToken;


                            const remoteAccessStatus           = remoteAccessInfoDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            remoteAccessStatus.className       = "row";

                            const remoteAccessStatusKey        = remoteAccessStatus.appendChild(document.createElement('div')) as HTMLDivElement;
                            remoteAccessStatusKey.className    = "subkey";
                            remoteAccessStatusKey.innerHTML    = "Status";

                            const remoteAccessStatusValue      = remoteAccessStatus.appendChild(document.createElement('div')) as HTMLDivElement;
                            remoteAccessStatusValue.className  = "value";

                            const remoteAccessStatusInput      = remoteAccessStatusValue.appendChild(document.createElement('input')) as HTMLInputElement;
                            remoteAccessStatusInput.value      = remoteAccessInfo.status;

                        }

                    }

                    //if (remoteParty.accessInfos !== undefined &&
                    //    remoteParty.accessInfos.length > 0) {

                    //    for (const accessInfo of remoteParty.accessInfos) {

                    //        const accessInfoDiv      = accessInfos.appendChild(document.createElement('div')) as HTMLDivElement;
                    //        accessInfoDiv.className  = "accessInfo";


                    //        const accessToken            = accessInfoDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    //        accessToken.className        = "row";

                    //        const accessTokenKey         = accessToken.appendChild(document.createElement('div')) as HTMLDivElement;
                    //        accessTokenKey.className     = "subkey";
                    //        accessTokenKey.innerHTML     = "Access Token";

                    //        const accessTokenValue       = accessToken.appendChild(document.createElement('div')) as HTMLDivElement;
                    //        accessTokenValue.className   = "value";

                    //        const accessTokenInput       = accessTokenValue.appendChild(document.createElement('input')) as HTMLInputElement;
                    //        accessTokenInput.value       = accessInfo.token;


                    //        const accessStatus           = accessInfoDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                    //        accessStatus.className       = "row";

                    //        const accessStatusKey        = accessStatus.appendChild(document.createElement('div')) as HTMLDivElement;
                    //        accessStatusKey.className    = "subkey";
                    //        accessStatusKey.innerHTML    = "Status";

                    //        const accessStatusValue      = accessStatus.appendChild(document.createElement('div')) as HTMLDivElement;
                    //        accessStatusValue.className  = "value";

                    //        const accessStatusInput      = accessStatusValue.appendChild(document.createElement('input')) as HTMLInputElement;
                    //        accessStatusInput.value      = accessInfo.status;

                    //    }

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
