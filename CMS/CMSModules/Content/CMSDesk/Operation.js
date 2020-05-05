// Refresh action
function RefreshTree(nodeId, selectNodeId) {
    if (parent != null) {
        if (parent.RefreshTree != null) {
            parent.RefreshTree(nodeId, selectNodeId);
        }
    }
}

// Selects the node within the tree
function SelectNode(nodeId) {
    if (parent != null) {
        if (parent.SelectNode != null) {
            parent.SelectNode(nodeId);
        }
    }
}