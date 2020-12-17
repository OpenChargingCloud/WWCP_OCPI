//var HTTPCookieId: string = "OCPIWebAPI";
//var mapboxgl = null;
function EncodeToken(str) {
    var buf = [];
    for (let i = str.length - 1; i >= 0; i--) {
        buf.unshift(['&#', str[i].charCodeAt(), ';'].join(''));
    }
    return buf.join('');
}
//# sourceMappingURL=defaults.js.map