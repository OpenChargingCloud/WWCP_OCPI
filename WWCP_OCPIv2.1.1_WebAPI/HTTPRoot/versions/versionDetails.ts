/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
///<reference path="../defaults/defaults.ts" />

function StartVersionDetails(versionId: string) {

    const common                               = GetDefaults();
    common.topLeft.innerHTML                   = "/version/details"
    common.menuVersions.classList.add("activated");
    common.menuVersions.href                   = "../versions";

    const versionDetailInfosDiv                = document.getElementById("versionDetailInfos")                     as HTMLDivElement;

    const accessTokenEncodingCheck             = versionDetailInfosDiv.querySelector("#accessTokenEncodingCheck")  as HTMLInputElement;
    const accessTokenInput                     = versionDetailInfosDiv.querySelector("#accessTokenInput")          as HTMLInputElement;
    const accessTokenButton                    = versionDetailInfosDiv.querySelector("#accessTokenButton")         as HTMLButtonElement;

    const versionDetailsDiv                    = versionDetailInfosDiv.querySelector("#versionDetails")            as HTMLDivElement;
    const versionIdDiv                         = versionDetailsDiv.    querySelector("#versionId")                 as HTMLDivElement;
    const endpointsDiv                         = versionDetailsDiv.    querySelector("#endpoints")                 as HTMLDivElement;

    accessTokenButton.onclick = () => {

        if (accessTokenEncodingCheck.checked)
            localStorage.setItem("ocpiAccessTokenEncoding", "base64");
        else
            localStorage.removeItem("ocpiAccessTokenEncoding");


        var newAccessToken = accessTokenInput.value;

        if (newAccessToken !== "")
            localStorage.setItem("ocpiAccessToken", newAccessToken);
        else
            localStorage.removeItem("ocpiAccessToken");


        location.reload();

    };

    OCPIGet(window.location.href, // == "/versions/2.2"

            (status, response) => {

                try
                {

                    const ocpiResponse = JSON.parse(response) as IOCPIResponse;

                    if (ocpiResponse?.data != undefined &&
                        ocpiResponse?.data != null)
                    {

                        const versionDetail = ocpiResponse.data as IVersionDetail;

                        versionIdDiv.innerHTML = "Version " + versionDetail.version;

                        for (const endpoint of versionDetail.endpoints) {

                            const endpointDiv      = endpointsDiv.appendChild(document.createElement('a')) as HTMLAnchorElement;
                            endpointDiv.className  = "endpoint";
                            endpointDiv.href       = endpoint.url;
                            endpointDiv.innerHTML  = endpoint.identifier + (versionDetail.version.startsWith("2.2") ? "/" + endpoint.role : "") + "<br /><span class=\"url\">" + endpoint.url + "</span>";

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
