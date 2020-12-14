///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

function StartVersions()
{

    const versionInfosDiv  = document.getElementById("versionInfos")       as HTMLDivElement;
    const versionsDiv      = versionInfosDiv.querySelector("#versions")    as HTMLDivElement;

    HTTPGet("/versions",

        (status, response) => {

            try
            {

                const OCPIResponse = ParseJSON_LD<IOCPIResponse>(response);

                if (OCPIResponse?.data != undefined  &&
                    OCPIResponse?.data != null       &&
                    Array.isArray(OCPIResponse.data) &&
                    OCPIResponse.data.length > 0)
                {

                    for (const version of (OCPIResponse.data as IVersion[])) {

                        const versionIdDiv      = versionsDiv.appendChild(document.createElement('a')) as HTMLAnchorElement;
                        versionIdDiv.className  = "version";
                        versionIdDiv.href       = version.url;
                        versionIdDiv.innerHTML  = "Version " + version.version + "<br /><span class=\"versionLink\">" + version.url + "</span>";

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
