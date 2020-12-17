
//var HTTPCookieId: string = "OCPIWebAPI";
//var mapboxgl = null;

interface IOCPIResponse {
    data:                   any;
    statusCode:             number;
    statusMessage:          string;
}

interface ISearchResult<T> {
    totalCount:             number;
    filteredCount:          number;
    searchResults:          Array<T>;
}

interface IVersion {
    version:                string;
    url:                    string;
}

interface IVersionDetails {
    version:                string;
    endpoints:              Array<IEndpoints>;
}

interface IEndpoints {
    identifier:             string;
    role:                   string;
    url:                    string;
}

interface IRemoteParty {
    "@id":                  string;
    "@context":             string;
    countryCode:            string;
    partyId:                string;
    role:                   string;
    businessDetails:        IBusinessDetails;
    partyStatus:            string;
    accessInfos:            Array<IAccessInfo>;
    remoteAccessInfos:      Array<IRemoteAccessInfo>;
    last_updated:           string;
}

interface IBusinessDetails {
    name:                   string;
    website:                string;
    logo:                   string;
}

interface IAccessInfo {
    token:                  string;
    status:                 string;
}

interface IRemoteAccessInfo {
    token:                  string;
    versionsURL:            string;
    versionIds:             Array<string>;
    selectedVersionId:      string;
    status:                 string;
}


function EncodeToken(str) {

    var buf = [];

    for (let i = str.length - 1; i >= 0; i--) {
        buf.unshift(['&#', str[i].charCodeAt(), ';'].join(''));
    }

    return buf.join('');
}
