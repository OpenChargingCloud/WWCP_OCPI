///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
function StartVersions() {
    const versionInfosDiv = document.getElementById("versionInfos");
    const versionsDiv = versionInfosDiv.querySelector("#versions");
    OCPIGet(window.location.href, // == "/versions"
    (status, response) => {
        try {
            const OCPIResponse = ParseJSON_LD(response);
            if ((OCPIResponse === null || OCPIResponse === void 0 ? void 0 : OCPIResponse.data) != undefined &&
                (OCPIResponse === null || OCPIResponse === void 0 ? void 0 : OCPIResponse.data) != null &&
                Array.isArray(OCPIResponse.data) &&
                OCPIResponse.data.length > 0) {
                for (const version of OCPIResponse.data) {
                    const versionIdDiv = versionsDiv.appendChild(document.createElement('a'));
                    versionIdDiv.className = "version";
                    versionIdDiv.href = version.url;
                    versionIdDiv.innerHTML = "Version " + version.version + "<br /><span class=\"versionLink\">" + version.url + "</span>";
                }
            }
        }
        catch (exception) {
        }
    }, (status, statusText, response) => {
    });
    //var refresh = setTimeout(StartDashboard, 30000);
}
//# sourceMappingURL=versions.js.map