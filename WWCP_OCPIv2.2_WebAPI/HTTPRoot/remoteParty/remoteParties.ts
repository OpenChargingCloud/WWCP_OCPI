///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

function StartRemoteParties()
{

    const remotePartiesInfosDiv  = document.getElementById("remotePartiesInfos")            as HTMLDivElement;
    const remotePartiesDiv       = remotePartiesInfosDiv.querySelector("#remoteParties")    as HTMLDivElement;

    HTTPGet("remoteParties?withMetadata",

        (status, response) => {

            try
            {

                const results = ParseJSON_LD<ISearchResult<IRemoteParty>>(response);

                if (results?.searchResults != undefined  &&
                    results?.searchResults != null       &&
                    Array.isArray(results.searchResults) &&
                    results.searchResults.length > 0)
                {

                    for (const remoteParty of results.searchResults) {

                        const remotePartyDiv        = remotePartiesDiv.appendChild(document.createElement('a')) as HTMLAnchorElement;
                        remotePartyDiv.className    = "remoteParty";
                        remotePartyDiv.href         = "remoteParties/" + remoteParty["@id"];

                        const partyHeadlineDiv      = remotePartyDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        partyHeadlineDiv.className  = "partyHeadline";
                        partyHeadlineDiv.innerHTML  = remoteParty.countryCode + "-" + remoteParty.partyId + " (" + remoteParty.role + ") " + remoteParty?.businessDetails?.name;

                        const remoteTokensDiv       = remotePartyDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        remoteTokensDiv.className   = "remoteTokens";
                        remoteTokensDiv.innerHTML   = remoteParty.remoteAccessInfos?.length > 0
                                                          ? (remoteParty.remoteAccessInfos.map(remoteAccessInfo => remoteAccessInfo.versionsURL + "<br />" +
                                                                                                                   "<i class=\"far fa-arrow-alt-circle-right\"></i>" + EncodeToken(remoteAccessInfo.token) + "<br />").
                                                            join())
                                                          : "";

                        const accessTokensDiv       = remotePartyDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                        accessTokensDiv.className   = "accessTokens";
                        accessTokensDiv.innerHTML   = remoteParty.accessInfos?.length > 0
                                                          ? (remoteParty.accessInfos.map(accessInfo => "<i class=\"far fa-arrow-alt-circle-left\"></i>" + EncodeToken(accessInfo.token) + "<br />").
                                                            join())
                                                          : "";

                    }

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
