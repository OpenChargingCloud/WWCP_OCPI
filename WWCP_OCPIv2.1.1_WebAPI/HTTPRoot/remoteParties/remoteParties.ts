///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

function StartRemoteParties()
{

    const common                = GetDefaults();
    common.topLeft.innerHTML    = "/Remote Parties"
    common.menuRemoteParties.classList.add("activated");
    common.menuVersions.href    = "../versions";

    OCPIStartSearch2<IRemotePartyMetadata, IRemoteParty>(

        window.location.href,
        () => {
            return "";
        },
        metadata => { },
        "remoteParty",
        remoteParty => remoteParty.countryCode + "-" + remoteParty.partyId + " (" + remoteParty.role + ")",
        "remoteParties",
        "remoteParties",

        // list view
        (resultCounter,
         remoteParty,
         remotePartyAnchor) => {

            const locationCounterDiv      = remotePartyAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            locationCounterDiv.className  = "counter";
            locationCounterDiv.innerHTML  = resultCounter.toString() + ".";

            //const remotePartyIdDiv        = remotePartyAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            //remotePartyIdDiv.className    = "id";
            //remotePartyIdDiv.innerHTML    = remoteParty.id

            const partyHeadlineDiv        = remotePartyAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            partyHeadlineDiv.className    = "partyHeadline";
            partyHeadlineDiv.innerHTML    = remoteParty.countryCode + "-" + remoteParty.partyId + " (" + remoteParty.role + ")";

            const propertiesDiv           = remotePartyAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            propertiesDiv.className       = "properties";

            CreateProperty(
                propertiesDiv,
                "status",
                "Status",
                remoteParty.partyStatus
            );

            CreateProperty(
                propertiesDiv,
                "businessDetails",
                "Business Details",
                remoteParty.businessDetails.website
                    ? "<a href=\"" + remoteParty.businessDetails.website + "\">" + remoteParty.businessDetails.name + "</a>"
                    : remoteParty.businessDetails.name
            );

            CreateProperty(
                propertiesDiv,
                "remoteTokens",
                "Remote Tokens",
                remoteParty.remoteAccessInfos?.length > 0
                    ? (remoteParty.remoteAccessInfos.map(remoteAccessInfo => remoteAccessInfo.versionsURL + "<br />" +
                        "<i class=\"far fa-arrow-alt-circle-right\"></i>" + EncodeToken(remoteAccessInfo.accessToken) + "<br />").
                        join())
                    : ""
            );

            CreateProperty(
                propertiesDiv,
                "accessTokens",
                "Access Tokens",
                remoteParty.localAccessInfos?.length > 0
                    ? (remoteParty.localAccessInfos.map(localAccessInfo => "<i class=\"far fa-arrow-alt-circle-left\"></i>" + EncodeToken(localAccessInfo.accessToken) + "<br />").
                        join())
                    : ""
            );


            const datesDiv      = remotePartyAnchor.appendChild(document.createElement('div')) as HTMLDivElement;
            datesDiv.className  = "dates properties";

            if (remoteParty.created)
                CreateProperty(
                    datesDiv,
                    "created",
                    "Created:",
                    remoteParty.created
                )

            CreateProperty(
                datesDiv,
                "lastUpdated",
                "Last updated:",
                remoteParty.last_updated
            )

        },

        // table view
        (remotePartys, remotePartysDiv) => {
        },

        // linkPrefix
        null,//remoteParty => "",
        searchResultsMode.listView,

        context => {
            //statusFilterSelect.onchange = () => {
            //    context.Search(true);
            //}
        }

    );

    //var refresh = setTimeout(StartDashboard, 30000);

}
