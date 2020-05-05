function modalDialog(url, name, width, height, otherParams, noWopener, forceModal, forceNewWindow, setTitle) {

    // Header and footer is greater than before, increase window size accordingly
    if (typeof(height) === "number") {
        height += 66;
    }

    // Set default parameter values
    if (setTitle == undefined) {
        setTitle = true;
    }
    if (forceModal == undefined) {
        forceModal = true;
    }
    if (otherParams == undefined) {
        otherParams = {
            toolbar: false,
            directories: false,
            menubar: false,
            modal: true,
            dependent: true,
            resizable: true,
            openInFullscreen: false
        };
    }

    var advanced = false;
    try {
        advanced = window.top.AdvancedModalDialogs;
    } catch (err) {
    }

    if (advanced && !forceNewWindow) {
        window.top.advancedModal(url, name, width, height, otherParams, noWopener, forceModal, setTitle, this);
    } else {
        var dHeight = height;
        var dWidth = width;
        if (width.toString().indexOf('%') != -1) {
            dWidth = Math.round(screen.width * parseInt(width, 10) / 100);
        }
        if (height.toString().indexOf('%') != -1) {
            dHeight = Math.round(screen.height * parseInt(height, 10) / 100);
        }

        var oWindow = window.open(url, name, 'width=' + dWidth + ',height=' + dHeight + ',' + otherParams);
        if (oWindow) {
            oWindow.opener = this;
            oWindow.focus();
        }
    }
}