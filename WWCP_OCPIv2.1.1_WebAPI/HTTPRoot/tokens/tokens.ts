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

function StartTokens()
{

    const common                       = GetDefaults();
    common.topLeft.innerHTML           = "/Tokens"
    common.menuVersions.classList.add("activated");
    common.menuVersions.href           = "../../versions";

    OCPIStartSearch2<ITokenMetadata, IToken>(

        window.location.href,
        () => {
            return "";
        },
        metadata => { },
        "token",
        token => token.uid,
        "tokens",
        "tokens",

        // list view
        (resultCounter,
         token,
         tokenAnchor) => {

            const locationCounterDiv      = tokenAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            locationCounterDiv.className  = "counter";
            locationCounterDiv.innerHTML  = resultCounter.toString() + ".";

            const tokenIdDiv              = tokenAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            tokenIdDiv.className          = "uid";
            tokenIdDiv.innerHTML          = token.uid;

            const propertiesDiv           = tokenAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            propertiesDiv.className       = "properties";

            CreateProperty(
                propertiesDiv,
                "type",
                "Type",
                token.type
            )

            CreateProperty(
                propertiesDiv,
                "authId",
                "Auth Id",
                token.auth_id
            )

            if (token.visual_number) {
                CreateProperty(
                    propertiesDiv,
                    "visualNumber",
                    "Visual Number",
                    token.visual_number
                )
            }

            CreateProperty(
                propertiesDiv,
                "issuer",
                "Issuer",
                token.issuer
            )

            CreateProperty(
                propertiesDiv,
                "isValid",
                "Is Valid",
                token.valid ? "valid" : "invalid"
            )

            CreateProperty(
                propertiesDiv,
                "whitelist",
                "Whitelist",
                token.whitelist
            )

            if (token.language) {
                CreateProperty(
                    propertiesDiv,
                    "language",
                    "Language",
                    token.language
                )
            }

            CreateProperty(
                propertiesDiv,
                "type",
                "Type",
                token.type
            )


            const datesDiv      = tokenAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            datesDiv.className  = "dates properties";

            if (token.created)
                CreateProperty(
                    datesDiv,
                    "created",
                    "Created:",
                    token.created
                )

            CreateProperty(
                datesDiv,
                "lastUpdated",
                "Last updated:",
                token.last_updated
            )

        },

        // table view
        (tokens, tokensDiv) => {
        },

        // linkPrefix
        null,//token => "",
        SearchResultsMode.listView,

        context => {
            //statusFilterSelect.onchange = () => {
            //    context.Search(true);
            //}
        }

    );

}
