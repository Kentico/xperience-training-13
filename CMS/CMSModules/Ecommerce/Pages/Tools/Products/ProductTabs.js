var IsCMSDesk = true;

function wopenerReload() {
    if (wopener) {
        var url = wopener.location.href;
        if (url.indexOf("cartexist=1") === -1) {
            url += "&cartexist=1";
        }
        wopener.location.replace(url);
        return true;
    } else {
        return false;
    }
};

function RefreshTree(expandNodeId, selectNodeId) {
    parent.RefreshTree(expandNodeId, selectNodeId);
}

function SelectNode(selectNodeId) {
    parent.SelectNode(selectNodeId);
}

function SetMode(mode, passive) {
    parent.SetMode(mode, passive);
}

function CloseSplitMode() {
    if (parent != this) {
        parent.CloseSplitMode();
    }
}

function PerformSplitViewRedirect(originalUrl, newCulture, successCallback, errorCallback, mode) {
    if (parent != this) {
        parent.PerformSplitViewRedirect(originalUrl, newCulture, successCallback, errorCallback, mode);
    }
}