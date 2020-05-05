var currentFolderId = 0;
var currentFolder = null;

// Select node action
function SelectPath(path) {
    // Get folder
    var folderToSelect = GetFolder(path);

    // Select folder
    SelectFolder(path, folderToSelect);

    FolderSelected();
}

// Select node action
function SelectFolder(folderId, folderElem) {
    // Get current node information
    currentFolderId = GetCurrentFolderID();
    currentFolder = GetFolder(currentFolderId);

    // If some node already selected - remove selection
    if ((currentFolder != null) && (currentFolder.length > 0) &&
            (folderElem != null) && (folderId != currentFolderId)) {
        currentFolder.removeClass('ContentTreeSelectedItem');
    }

    // Update current folder information
    currentFolderId = folderId;

    if (folderElem != null) {
        currentFolder = folderElem;
        if ((currentFolder != null) && (currentFolder.length > 0)) {
            currentFolder.addClass('ContentTreeSelectedItem');
        }
    }
}

// Returns object representing current node
function GetFolder(folderId) {
    if (folderId != 0) {
        return $cmsj("#" + folderId.toLowerCase());
    }
    return null;
}

// Handles bug present in the .NET 2.0 when combination of TreeView and UpdatePanels causes problems with ViewState of the TreeView
function ResolveDuplicateNodes(clientId) {
    // Following code replaces ID of the firstly rendered node so there are no more identic IDs of the rendered nodes
    var $expandLink = $cmsj("a[id$='n1']");
    if (($expandLink != null) && ($expandLink.length > 0)) {
        var altId = "99999"; // Random number high enough used as ID suffix

        var origId = $expandLink.attr("id");
        var newId = origId.replace(/1$/, altId);

        $expandLink.attr('id', newId);

        var origHref = $expandLink.attr('href');
        var newHref = origHref.replace(/,1,/gi, "," + altId + ",").replace(clientId + "n1", clientId + "n" + altId).replace(clientId + "t1", clientId + "t" + altId);

        $expandLink.attr('href', newHref);

        var $folderLink = $cmsj("span[id$='t1']");
        if (($folderLink != null) && ($folderLink.length > 0)) {
            origId = $folderLink.attr("id");
            newId = origId.replace("1", altId);

            $folderLink.attr('id', newId);
        }
    }
}

function SetLibParentAction(argument) {
    // Raise select action
    SetAction('morefolderselect', argument);
    RaiseHiddenPostBack();
}