var IsCMSDesk = true;

function RefreshTree(expandNodeId, selectNodeId) {
    // Update tree
    if (parent != this) {
        parent.RefreshTree(expandNodeId, selectNodeId);
    }
    return false;
}

function SelectNode(nodeId, nodeElem, tab) {
    if (parent != this) {
        parent.SelectNode(nodeId, nodeElem, tab);
    }
}

function SetSelectedMode(mode) {
    if (parent != this) {
        parent.SetSelectedMode(mode);
    }
}

function SetMode(mode, passive) {
    if (parent != this) {
        parent.SetMode(mode, passive);
    }
}

function NewDocument(parentNodeId, className) {
    if ((parentNodeId != 0) && (parent != this)) {
        parent.NewDocument(parentNodeId, className);
        parent.RefreshTree(parentNodeId, parentNodeId);
    }
}

function DeleteDocument(nodeId) {
    if ((nodeId != 0) && (parent != this)) {
        parent.DeleteDocument(nodeId);
        parent.RefreshTree(nodeId, nodeId);
    }
}

function EditDocument(nodeId, tab, culture) {
    if ((nodeId != 0) && (parent != this)) {    
      if (culture) {
        parent.ChangeLanguage(culture);
      }
      parent.EditDocument(nodeId, tab, culture);
      parent.RefreshTree(nodeId, nodeId);
    }
}

// Refresh tree with current node selected
function TreeRefresh() {
    if (parent != this) {
        parent.TreeRefresh();
    }
}

function CheckChanges() {
    if (window.frames['c'] && (window.frames['c'].CheckChanges)) {
        return window.frames['c'].CheckChanges();
    }
    return true;
}

function CloseSplitMode() {
    if (parent != this) {
        parent.CloseSplitMode();
    }
}

function SplitModeRefreshFrame() {
    if (parent != this) {
        parent.SplitModeRefreshFrame();
    }
}

// Refresh selected device
function ChangeDevice(device) {
    if ((parent != this) && parent.ChangeDevice) {
        parent.ChangeDevice(device);
    }
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
