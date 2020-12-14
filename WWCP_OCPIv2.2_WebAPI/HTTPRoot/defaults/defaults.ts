
//var HTTPCookieId: string = "OCPIWebAPI";
//var mapboxgl = null;

interface IOCPIResponse {
    data:           any;
    statusCode:     number;
    statusMessage:  string;
}


interface IVersion {
    version:        string;
    url:            string;
}

interface IVersionDetails {
    version:        string;
    endpoints:      Array<IEndpoints>;
}

interface IEndpoints {
    identifier:     string;
    role:           string;
    url:            string;
}
