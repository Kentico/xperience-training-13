function RefreshTree(expandNodeId, selectNodeId) {
    if (parent.RefreshTree) {
        // Update tree
        parent.RefreshTree(expandNodeId, selectNodeId);
    }
}

function CheckChanges() {
    if (window.frames['c'].CheckChanges) {
        return window.frames['c'].CheckChanges();
    }
    return true;
}

function SelectNode(nodeId) {
    if (parent.SelectNode) {
        parent.SelectNode(nodeId);
    }
}

function RefreshLeftMenu() {
    window.location.replace(window.location.href);
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
