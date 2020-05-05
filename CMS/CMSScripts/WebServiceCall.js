var WebServiceCall = (function () {

    function GetXmlHTTPRequest() {
        try { return new XMLHttpRequest(); } catch (e) { }
        try { return new ActiveXObject('Msxml2.XMLHTTP'); } catch (ex) { }
        return null;
    }

    return function (url, method, parameters) {
        var r = GetXmlHTTPRequest();
        r.open('POST', url + '/' + method, true);
        r.setRequestHeader('Content-type', 'application/json');
        r.setRequestHeader('Accept', 'application/json');
        r.send(parameters);
    };
}());