
function InsertSelectedItem(obj) {
    if ((window.wopener) && (obj)) {
        var url = null;
        if ((obj.img_url) && (obj.img_url != '')) {
            url = createUrl(obj.img_url, obj.img_ext, obj.img_width, obj.img_height);
        }
        else if ((obj.av_url) && (obj.av_url != '')) {
            url = createUrl(obj.av_url, obj.av_ext, obj.av_width, obj.av_height);
        }
        else if ((obj.url_url) && (obj.url_url != '')) {
            url = createUrl(obj.url_url, obj.url_ext, obj.url_width, obj.url_height);
        }

        var hdnAlt = window.wopener.document.getElementById(obj.imgalt_clientid);
        if (hdnAlt != null) {
            hdnAlt.value = obj.img_alt;
        }

        if ((obj.editor_clientid != null) && (obj.editor_clientid != '')) {
            var editor = window.wopener.document.getElementById(obj.editor_clientid);
            if (editor != null) {
                if (url != null) {
                    if (editor.value != null) {
                        editor.value = url;
                        if (editor.onchange) {
                            editor.onchange();
                        }
                    }
                }
            }
        }
        else if (window.wopener.SetUrl != null) {
            window.wopener.SetUrl(url, obj.url_width, obj.url_height);
        }
        // Insert url to default CKEDITOR dialog by FileBrowser API
	    // Used in Image and Link dialogs 
        // http://docs.ckeditor.com/#!/guide/dev_file_browser_api
        else if (window.wopener.CKEDITOR) {
        	var funcNum = getUrlParam('CKEditorFuncNum');
        	window.wopener.CKEDITOR.tools.callFunction(funcNum, url);
        }
    }
}

// Helper function to get parameters from the query string.
function getUrlParam(paramName) {
	var reParam = new RegExp('(?:[\?&]|&)' + paramName + '=([^&]+)', 'i');
	var match = window.location.search.match(reParam);

	return (match && match.length > 1) ? match[1] : null;
}


function GetSelectedItem(editorId) {
    var obj = null;
    if ((editorId) && (editorId != '')) {
        if (window.wopener) {
            var editor = window.wopener.document.getElementById(editorId);
            if ((editor != null) && (editor.value) && (editor.value != '')) {
                obj = new Object();
                obj.url_url = editor.value;
                var ext = editor.value.match(/ext=([^&]*)/);
                if (ext) {
                    obj.url_ext = ext[1];
                }
                var width = editor.value.match(/width=([^&]*)/);
                if (width) {
                    obj.url_width = width[1];
                }
                var height = editor.value.match(/height=([^&]*)/);
                if (height) {
                    obj.url_height = height[1];
                }
            }
        }
    }
    return obj;
}

function createUrl(url, ext, width, height) {
    /*
    var query = '';
    // Create query string
    if ((ext) && (ext != '')) {
    query += "&ext=" + ext;
    }
    if ((width) && (width != '')) {
    query += "&width=" + width;
    }
    if ((height) && (height != '')) {
    query += "&height=" + height;
    }
    // Add query string into url
    if (url.lastIndexOf('?') > 0) {
    url = url + query;
    }
    else {
    url = url + '?' + query.replace(/^&/, '');
    }
    */
    return url;
}
