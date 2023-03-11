
//var HTTPCookieId: string = "OCPIWebAPI";
//var mapboxgl = null;

interface IOCPIResponse {
    data:                            any;
    statusCode:                      number;
    statusMessage:                   string;
}

interface ISearchResult<T> {         
    totalCount:                      number;
    filteredCount:                   number;
    searchResults:                   Array<T>;
}

interface IVersion {
    version:                         string;
    url:                             string;
}

interface IVersionDetails {
    version:                         string;
    endpoints:                       Array<IEndpoints>;
}

interface IEndpoints {
    identifier:                      string;
    role:                            string;
    url:                             string;
}

interface ILocation {
    country_code:                    string;
    party_id:                        string;
    id:                              string;
    location_type:                   string;
    name?:                           string;
    address:                         string;
    city:                            string;
    postal_code:                     string;
    country:                         string;
    coordinates:                     ICoordinates;
    related_locations?:              Array<IAdditionalGeoLocation>;
    evses?:                          Array<IEVSE>;
    directions?:                     Array<IDisplayText>;
    operator?:                       IBusinessDetails;
    suboperator?:                    IBusinessDetails;
    owner?:                          IBusinessDetails;
    facilities?:                     Array<string>;
    time_zone?:                      string;
    opening_times?:                  string;
    charging_when_closed?:           boolean;
    images?:                         Array<IImage>;
    energy_mix?:                     IEnergyMix;
    publish?:                        boolean;                            // A PlugSurfing extension!
    last_updated:                    string;
}

interface ICoordinates {
    latitude:                        string;
    longitude:                       string;
}

interface IAdditionalGeoLocation {
    latitude:                        string;
    longitude:                       string;
    name?:                           IDisplayText
}

interface IDisplayText {
    language:                        string;
    text:                            string;
}

interface IBusinessDetails {         
    name:                            string;
    website?:                        string;
    logo?:                           string;
}

interface IImage {
    url:                             string;
    category:                        string;
    type:                            string;
    thumbnail?:                      string;
    width?:                          string;
    height?:                         string;
}

interface IEnergyMix {
    is_green_energy:                 boolean;
    energy_sources?:                 Array<IEnergySource>;
    environ_impact?:                 Array<IEnvironmentalImpact>;
    supplier_name?:                  string;
    energy_product_name?:            string;
}

interface IEnergySource {
    source:                          string;
    percentage:                      number;
}

interface IEnvironmentalImpact {
    category:                        string;
    amount:                          number;
}

interface IEVSE {
    uid:                             string;
    evse_id?:                        string;
    status:                          string;
    status_schedule?:                Array<IStatusSchedule>;
    capabilities?:                   Array<string>;
    connectors:                      Array<IConnector>;
    energy_meter?:                   IEnergyMeter;                      // OCPI Computer Science extension!
    floor_level?:                    string;
    coordinates:                     ICoordinates;
    physical_reference?:             string;
    directions?:                     Array<IDisplayText>;
    parking_restrictions?:           Array<string>;
    images?:                         Array<IImage>;
    last_updated:                    string;
}

interface IStatusSchedule {
    period_begin:                    string;
    period_end?:                     string;
    status:                          string;
}

interface IConnector {
    id:                              string;
    standard:                        string;
    format:                          string;
    power_type:                      string;
    voltage:                         number;
    amperage:                        number;
    tariff_id?:                      string;
    terms_and_conditions?:           string;
    last_updated:                    string;
}

// OCPI Computer Science extension!
interface IEnergyMeter {
    id:                              string;
    model?:                          string;
    model_url?:                      string;
    hardware_version?:               string;
    firmware_version?:               string;
    manufacturer?:                   string;
    manufacturer_url?:               string;
    public_keys?:                    Array<string>;
    public_key_certificate_chain?:   string;
    transparency_softwares?:         Array<ITransparencySoftwareStatus>;

}

// OCPI Computer Science extension!
interface ITransparencySoftwareStatus {
    transparency_software:           ITransparencySoftware;
    legal_status:                    string;
    certificate:                     string;
    certificate_issuer:              string;
    not_before:                      string;
    not_after:                       string;
}

// OCPI Computer Science extension!
interface ITransparencySoftware {
    name:                            string;
    version:                         string;
    open_source_license:             Array<IOpenSourceLicense>;
    vendor:                          string;
    logo?:                           string;
    how_to_use?:                     string;
    more_information?:               string;
    source_code_repository?:         string;
}

// OCPI Computer Science extension!
interface IOpenSourceLicense {
    id:                              string;
    description?:                    Array<IDisplayText>;
    urls?:                           Array<string>;
}

interface IOpenDataLicense {
    id:                              string;
    description?:                    Array<IDisplayText>;
    urls?:                           Array<string>;
}


interface IRemoteParty {
    "@id":                           string;
    "@context":                      string;
    countryCode:                     string;
    partyId:                         string;
    role:                            string;
    businessDetails:                 IBusinessDetails;
    partyStatus:                     string;
    accessInfos:                     Array<IAccessInfo>;
    remoteAccessInfos:               Array<IRemoteAccessInfo>;
    last_updated:                    string;
}



interface IAccessInfo {
    token:                           string;
    status:                          string;
}

interface IRemoteAccessInfo {
    token:                           string;
    versionsURL:                     string;
    versionIds:                      Array<string>;
    selectedVersionId:               string;
    status:                          string;
}


function EncodeToken(str) {

    var buf = [];

    for (let i = str.length - 1; i >= 0; i--) {
        buf.unshift(['&#', str[i].charCodeAt(), ';'].join(''));
    }

    return buf.join('');
}


// #region OCPIGet(RessourceURI, AccessToken, OnSuccess, OnError)

function OCPIGet(RessourceURI: string,
                 OnSuccess,
                 OnError) {

    const ajax = new XMLHttpRequest();
    ajax.open("GET", RessourceURI, true);
    ajax.setRequestHeader("Accept",   "application/json; charset=UTF-8");
    ajax.setRequestHeader("X-Portal", "true");

    if (localStorage.getItem("OCPIAccessToken") != null)
        ajax.setRequestHeader("Authorization", "Token " + btoa(localStorage.getItem("OCPIAccessToken")));

    ajax.onreadystatechange = function () {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            // Ok
            if (this.status >= 100 && this.status < 300) {

                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));

                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(this.status, ajax.responseText);

            }

            else
                if (OnError && typeof OnError === 'function')
                    OnError(this.status, this.statusText, ajax.responseText);

        }

    }

    ajax.send();

}

// #endregion

