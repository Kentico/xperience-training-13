var cmsListingContentApp = '';

// Delete item action
function DeleteItem(nodeId) {
    if (nodeId > 0) {
        location.href = '../Delete.aspx?multiple=true&nodeid=' + nodeId;
    }
}

// Edit item action
function EditItem(nodeId, parentNodeId) {
    if (nodeId > 0) {
        parent.SetMode('edit', true);
        parent.SelectNode(nodeId);
        parent.RefreshTree(nodeId, nodeId);
    }
}

// Select item action
function SelectItem(nodeId) {
    if (nodeId > 0) {
        parent.SetMode('listing', true);
        parent.SelectNode(nodeId);
        parent.RefreshTree(nodeId, nodeId);
    }
}

// Redirect item
function RedirectItem(nodeId, culture) {
    if (parent != null) {
        if (parent.parent != null) {
            if (parent.parent.parent != null) {
                parent.parent.parent.location.href = '../../../../Admin/CMSAdministration.aspx?action=edit&mode=editform&nodeid=' + nodeId + '&culture=' + culture + cmsListingContentApp;
            }
        }
    }
}