///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

function StartVersionDetails(versionId: string) {

    const versionDetailInfosDiv  = document.getElementById("versionDetailInfos")              as HTMLDivElement;

    const accessTokenInput       = versionDetailInfosDiv.querySelector("#accessToken")        as HTMLInputElement;
    const accessTokenButton      = versionDetailInfosDiv.querySelector("#accessTokenButton")  as HTMLButtonElement;

    const versionDetailsDiv      = versionDetailInfosDiv.querySelector("#versionDetails")     as HTMLDivElement;
    const versionIdDiv           = versionDetailsDiv.    querySelector("#versionId")          as HTMLDivElement;
    const endpointsDiv           = versionDetailsDiv.    querySelector("#endpoints")          as HTMLDivElement;

    accessTokenButton.onclick = () => {

        var newAccessToken = accessTokenInput.value;

        if (newAccessToken !== "")
            localStorage.setItem("OCPIAccessToken", newAccessToken);

        else
            localStorage.removeItem("OCPIAccessToken");

        location.reload();

    };

    OCPIGet(window.location.href, // == "/versions/2.2"

            (status, response) => {

                try
                {

                    const OCPIResponse = ParseJSON_LD<IOCPIResponse>(response);

                    if (OCPIResponse?.data != undefined &&
                        OCPIResponse?.data != null)
                    {

                        const versionDetails = OCPIResponse.data as IVersionDetails;

                        versionIdDiv.innerHTML = "Version " + versionDetails.version;

                        for (const endpoint of versionDetails.endpoints) {

                            const endpointDiv      = endpointsDiv.appendChild(document.createElement('a')) as HTMLAnchorElement;
                            endpointDiv.className  = "endpoint";
                            endpointDiv.href       = endpoint.url;
                            endpointDiv.innerHTML  = endpoint.identifier + "/" + endpoint.role + "<br /><span class=\"url\">" + endpoint.url + "</span>";

                        }

                    }

                }
                catch (exception) {
                }

            },

            (status, statusText, response) => {
            }

    );

}
