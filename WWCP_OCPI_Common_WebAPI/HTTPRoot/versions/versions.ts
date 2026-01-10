/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI WebAPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

///<reference path="../defaults/defaults.ts" />

function StartVersions()
{

    const common              = GetDefaults();
    common.topLeft.innerHTML  = "/versions"
    //common.menuVersions.classList.add("activated");

    const versionInfosDiv     = document.getElementById("versionInfos")     as HTMLDivElement;
    const versionsDiv         = versionInfosDiv.querySelector("#versions")  as HTMLDivElement;

    OCPIGet(window.location.href, // == "/versions"

            (status, response) => {

                try
                {

                    const ocpiResponse = JSON.parse(response) as IOCPIResponse;

                    if (ocpiResponse?.data != undefined  &&
                        ocpiResponse?.data !== null       &&
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

function StartVersionsOnIndexPage() {

    const common = GetDefaults();
    common.topLeft.innerHTML = "/"
    //common.menuVersions.classList.add("activated");

    const versionInfosDiv = document.getElementById("versionInfos") as HTMLDivElement;
    const versionsDiv = versionInfosDiv.querySelector("#versions") as HTMLDivElement;

    var versionsURL = window.location.href;
    if (!versionsURL.endsWith("/"))
        versionsURL += "/";

    versionsURL += "versions";

    OCPIGet(versionsURL,

        (status, response) => {

            try {

                const ocpiResponse = JSON.parse(response) as IOCPIResponse;

                if (ocpiResponse?.data != undefined &&
                    ocpiResponse?.data !== null &&
                    Array.isArray(ocpiResponse.data) &&
                    ocpiResponse.data.length > 0) {

                    for (const version of (ocpiResponse.data as IVersion[])) {

                        const versionIdDiv = versionsDiv.appendChild(document.createElement('a')) as HTMLAnchorElement;
                        versionIdDiv.className = "version";
                        versionIdDiv.href = version.url;
                        versionIdDiv.innerHTML = "Version " + version.version + "<br /><span class=\"versionLink\">" + version.url + "</span>";

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
