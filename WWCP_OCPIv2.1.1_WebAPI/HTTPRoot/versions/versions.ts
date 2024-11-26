///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
///<reference path="../defaults/defaults.ts" />

function StartVersions()
{

    const common              = GetDefaults();
    common.topLeft.innerHTML  = "/versions"
    common.menuVersions.classList.add("activated");

    const versionInfosDiv     = document.getElementById("versionInfos")     as HTMLDivElement;
    const versionsDiv         = versionInfosDiv.querySelector("#versions")  as HTMLDivElement;

    OCPIGet(window.location.href, // == "/versions"

            (status, response) => {

                try
                {

                    const ocpiResponse = JSON.parse(response) as IOCPIResponse;

                    if (ocpiResponse?.data != undefined  &&
                        ocpiResponse?.data != null       &&
                        Array.isArray(ocpiResponse.data) &&
                        ocpiResponse.data.length > 0)
                    {

                        for (const version of (ocpiResponse.data as IVersion[])) {

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
