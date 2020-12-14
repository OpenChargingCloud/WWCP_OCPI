///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
function StartVersionDetails(versionId) {
    const versionDetailInfosDiv = document.getElementById("versionDetailInfos");
    const versionDetailsDiv = versionDetailInfosDiv.querySelector("#versionDetails");
    const versionIdDiv = versionDetailsDiv.querySelector("#versionId");
    const endpointsDiv = versionDetailsDiv.querySelector("#endpoints");
    HTTPGet("/versions/2.2", (status, response) => {
        try {
            const OCPIResponse = ParseJSON_LD(response);
            if ((OCPIResponse === null || OCPIResponse === void 0 ? void 0 : OCPIResponse.data) != undefined &&
                (OCPIResponse === null || OCPIResponse === void 0 ? void 0 : OCPIResponse.data) != null) {
                const versionDetails = OCPIResponse.data;
                versionIdDiv.innerHTML = "Version " + versionDetails.version;
                for (const endpoint of versionDetails.endpoints) {
                    const endpointDiv = endpointsDiv.appendChild(document.createElement('a'));
                    endpointDiv.className = "endpoint";
                    endpointDiv.href = endpoint.url;
                    endpointDiv.innerHTML = endpoint.identifier + "/" + endpoint.role + "<br /><span class=\"url\">" + endpoint.url + "</span>";
                }
            }
        }
        catch (exception) {
        }
    }, (status, statusText, response) => {
    });
    //var refresh = setTimeout(StartDashboard, 30000);
}
//# sourceMappingURL=versionDetails.js.map