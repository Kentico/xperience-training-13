function initCrop() {
    var mainImage = $cmsj('img[id$=imgContent]');
    if (mainImage.length) {
        if ((mainImage.width() > 0) && (mainImage.height() > 0)) {
            if (parent.UpdateTrimCoords) {
                if (jcrop_api != null) {
                    jcrop_api.destroy();
                }
                jcrop_api = $cmsj.Jcrop(mainImage[0], {
                    onChange: parent.UpdateTrimCoords,
                    onSelect: parent.UpdateTrimCoords,
                    maxsize: [mainImage.width(), mainImage.height()]
                });
            }
            else {
                setTimeout(function() { initCrop(); }, 300);
            }
        }
        else {
            setTimeout(function() { initCrop(); }, 300);
        }
    }
    else {
        setTimeout(function() { initCrop(); }, 300);
    }
}

function destroyCrop() {
    if (jcrop_api) {
        jcrop_api.destroy();
    }
}

function resetCrop() {
    if (jcrop_api) {
        jcrop_api.release();
    }
}

function lockAspectRatio(lock, width, height) {
    if (jcrop_api != null) {
        if (lock) {
            if (height > 0) {
                jcrop_api.setOptions({ aspectRatio: (width / height) });
            }
            else {
                jcrop_api.setOptions({ aspectRatio: 1 });
            }
        }
        else {
            jcrop_api.setOptions({ aspectRatio: 0 });
        }
    }
}

function updateTrim(x1, y1, x2, y2) {
    if (jcrop_api != null) {
        jcrop_api.setSelect([x1, y1, x2, y2]);
    }
}