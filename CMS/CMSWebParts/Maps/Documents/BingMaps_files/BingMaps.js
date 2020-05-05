var enableSearchLogo = false;
var _mapInitializers = [];

function addBingMarker(map, latitude, longtitude, title, content, iconURL) {
    var offset = new Microsoft.Maps.Point(0, 5);
    if (iconURL) {
        var pushpinOptions = {
            icon: iconURL,
            textOffset: offset
        };
    }
    else {
        var pushpinOptions = {
            textOffset: offset
        };
    }

    var pushpin = new Microsoft.Maps.Pushpin(new Microsoft.Maps.Location(latitude, longtitude), pushpinOptions);
    pushpin.description = content;
    map.entities.push(pushpin);
    pushpin.title = title;
    return pushpin;
}
function showInfo(map, pushpin, zoom) {
    var location = pushpin.getLocation();
    var infoboxOptions = {
        title: pushpin.title, description: pushpin.description
    };
    var infobox = new Microsoft.Maps.Infobox(location, infoboxOptions);
    infobox.setMap(map);
    map.setView({
        zoom: zoom,
        center: location
    });
}

function callBingService(url) {
    var script = document.createElement("script");
    script.setAttribute("type", "text/javascript");
    script.setAttribute("src", url);
    document.body.appendChild(script);
}

function replaceContent(className, expression, replacement) {
    var selHTMLTags = new Array();
    var selHTMLTags = document.getElementsByClassName(className);
    for (i = 0; i < selHTMLTags.length; i++) {
        selHTMLTags[i].innerHTML = selHTMLTags[i].innerHTML.replace(expression, replacement);
    }
}

function registerMapInitializer(initializer) {
    _mapInitializers.push(initializer);
}

function loadMaps() {
    _mapInitializers.forEach(function (mapInit) {
        if (typeof mapInit === 'function') {
            mapInit();
        }
    });
}