function LoadYTVideo(elementId, url, width, height)
{
      var content = '<object type="application/x-shockwave-flash" data="' + url + '" width="' + width + '" height="' + height + '" id="VideoPlayback">' + 
            '<param name="classid" value="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" />\n' +
            '<param name="codebase" value="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,40,0" />\n' +
            '<param name="movie" value="'+ url +'" />' + 
            '<param name="allowScriptAcess" value="sameDomain" />' + 
            '<param name="quality" value="best" />' +
            '<param name="scale" value="noScale" />' +
            '<param name="pluginurl" value="http://www.adobe.com/go/getflashplayer" />\n' +
            '<param name="salign" value="TL" />' +
            '<param name="FlashVars" value="playerMode=embedded" />' + 
            '<param name="wmode" value="transparent"/>\n' +
            '</object>';

    var element = document.getElementById(elementId);
    element.innerHTML = content;
}