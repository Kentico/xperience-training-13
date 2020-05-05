// Select node in CMSDesk-Content tree
function SelectNode(nodeId) {
    if (window.wopener != null) {
        if (window.wopener.SelectNode != null) {
            window.wopener.SelectNode(nodeId);
        }
    }
}

// Refresh node in CMSDesk-Content tree
function RefreshTree(nodeId, selectNodeId) {
    if (window.wopener != null) {
        if (window.wopener.RefreshTree != null) {
            window.wopener.RefreshTree(nodeId, selectNodeId);
        }
    }
}

// Content tree refresh
function TreeRefresh() {
    if (window.wopener != null) {
        if (window.wopener.TreeRefresh != null) {
            window.wopener.TreeRefresh();
        }
    }
}

// Refresh listing after multiple action
function RefreshListing() {
    if (window.wopener != null) {
        if (window.wopener.RefreshGrid != null) {
            window.wopener.RefreshGrid();
        }
    }
}

// Initialize gray overlay in dialog
function InitializeLog() {
    var header = window.GetTop().frames['insertHeader'];
    var footer = window.GetTop().frames['insertFooter'];
    if (header != null) {
        var headerOverlay = header.document.createElement('DIV');
        headerOverlay.id = 'headerOverlay';
        headerOverlay.style.zIndex = '2500';
        headerOverlay.className = 'async-log-bg';
        header.document.body.insertBefore(headerOverlay, header.document.body.firstChild);
    }
    if (footer != null) {
        var footerOverlay = footer.document.createElement('DIV');
        footerOverlay.id = 'footerOverlay';
        footerOverlay.style.zIndex = '2500';
        footerOverlay.className = 'async-log-bg';
        footer.document.body.insertBefore(footerOverlay, footer.document.body.firstChild);
    }
    if (window.parent.expandIframe) {
        window.parent.expandIframe();
    }
}

// Remove gray overlay in dialog
function DestroyLog() {
    var header = window.GetTop().frames['insertHeader'];
    var footer = window.GetTop().frames['insertFooter'];
    if (header != null) {
        var headerOverlay = header.document.getElementById('headerOverlay');
        if (headerOverlay != null) {
            header.document.body.removeChild(headerOverlay);
        }
    }
    if (footer != null) {
        var footerOverlay = footer.document.getElementById('footerOverlay');
        if (footerOverlay != null) {
            footer.document.body.removeChild(footerOverlay);
        }
    }
    if (window.parent.collapseIframe) {
        window.parent.collapseIframe();
    }
}

// Clear UniGrid selection in opener window
function ClearSelection() {
    if (window.wopener != null) {
        if (window.wopener.ClearSelection) {
            window.wopener.ClearSelection();
        }
    }
}