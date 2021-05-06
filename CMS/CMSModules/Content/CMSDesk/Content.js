/*#region CURRENT CONTEXT */
var currentNodeId = 0;

function EnsureSelectedModeElem() {
    if (window.selModeElem) {
        return;
    }
    window.selModeElem = document.getElementById('selectedMode');
}

function GetSelectedMode() {
    EnsureSelectedModeElem();
    return selModeElem.value;
}

SetSelectedMode = function (mode) {
    EnsureSelectedModeElem();
    selModeElem.value = mode;

    cmsrequire(['CMS/EventHub'], function(EventHub) {
        EventHub.publish('Content.viewModeChanged', {
            mode: mode
        });
    });
};

function EnsureSelectedNodeIdElem() {
    if (window.selNodeElem) {
        return;
    }
    window.selNodeElem = document.getElementById('selectedNodeId');
}

function GetSelectedNodeId() {
    EnsureSelectedNodeIdElem();
    return selNodeElem.value;
}

function SetSelectedNodeId(nodeId) {
    EnsureSelectedNodeIdElem();
    selNodeElem.value = nodeId;
}

function EnsureSelectedCultureElem() {
    if (window.selCultureElem) {
        return;
    }
    window.selCultureElem = document.getElementById('selectedCulture');
}

function GetSelectedCulture() {
    EnsureSelectedCultureElem();
    return selCultureElem.value;
}

function SetSelectedCulture(culture) {
    EnsureSelectedCultureElem();
    selCultureElem.value = culture;
}

function EnsureSelectedSplitModeCultureElem() {
    if (window.selSplitModeCultureElem) {
        return;
    }
    window.selSplitModeCultureElem = document.getElementById('selectedSplitModeCulture');
}

function GetSelectedSplitModeCulture() {
    EnsureSelectedSplitModeCultureElem();
    return selSplitModeCultureElem.value;
}

function SetSelectedSplitModeCulture(splitModeCulture) {
    EnsureSelectedSplitModeCultureElem();
    selSplitModeCultureElem.value = splitModeCulture;
}

//#endregion

/*#region GENERAL OPERATIONS*/

// Finds out whether data were changed
function CheckChanges() {
    try {
        if (frames['contentview'].CheckChanges) {
            return frames['contentview'].CheckChanges();
        }
    }
    catch (ex) {
    }

    return true;
}


// Mode set action - sets the current editing mode
function SetMode(mode, passive) {
    if (!CheckChanges()) {
        return false;
    }
    SetSelectedMode(mode);
    if (!passive) {
        DisplayDocument();
    }
    return true;
}

//#endregion

/*#region SPLITVIEW MODE*/

// Changes the split view
function ChangeSplitMode(elem) {
    if (!CheckChanges()) {
        return false;
    }

    var newCookieValue = "1|#|Vertical|1";

    try {
        var cookieValue = $cmsj.cookie('CMSSplitMode');
        var values = cookieValue.split('|');
        if (values.length == 4) {
            var displaySplitMode = values[0];
            if (displaySplitMode == '1') {
                displaySplitMode = '0';
            }
            else {
                displaySplitMode = '1';
            }
            // Display split mode|split mode culture|split mode layout|synchronize scrollbars
            newCookieValue = displaySplitMode + "|" + values[1] + "|" + values[2] + "|" + values[3];
        }
    }
    catch (e) {
        // Use default value
    }

    // Set new values to cookies
    $cmsj.cookie("CMSSplitMode", newCookieValue.toString(), { expires: 365, path: '/' });
    SelectNode(GetSelectedNodeId());
    $cmsj(elem).toggleClass('active');
    return true;
}

// Close the split view
function CloseSplitMode() {
    if (!CheckChanges()) {
        return false;
    }

    // Click the toggle button
    $cmsj(".js-split-toggle").click();
    return true;
}

function PerformSplitViewRedirect(originalUrl, newCulture, successCallback, errorCallback, mode) {
    PerformRedirect(mode, null, GetSelectedNodeId(), newCulture, null, originalUrl, true, successCallback, errorCallback);
}

//#endregion

// Switches to editing mode of particular document
function EditDocument(nodeId, tab, culture) {
    SetMode('editform', true);

    var query = null;
    if (tab != null) {
        var parts = tab.split("|");
        if (parts.length > 1) {
            query = '?tabName=' + parts[0] + '&subtab=' + parts[1];
        }
        else {
            query = '?tabName=' + tab;
        }
    }

    window.PerformContentRedirect('editform', 'content', nodeId, query, culture);
}

// Displays the selected document in the current mode
function DisplayDocument(tab) {
    var mode = GetSelectedMode();
    var nodeId = GetSelectedNodeId();
    var action = '';
    if (mode == 'edit') {
        action = 'content';
    }
    var query = '';
    if (tab != null) {
        query = '?tabName=' + tab;
    }

    window.PerformContentRedirect(mode, action, nodeId, query);
}

// Not allowed action
function NotAllowed() {
}

// New document action
function NewDocument(parentNodeId, classId) {
    if ((parentNodeId != 0) && (classId != 0)) {
        NewItem(parentNodeId, classId, false, null);
    }

}

// Particular document delete action
function DeleteDocument(nodeId) {
    if (nodeId > 0) {
        SetSelectedNodeId(nodeId);
        DeleteItem();
    }
}

/*#region CONTENT ACTIONS*/
// New item action
function NewItem(nodeId, classId, refreshTree, query) {
    if (!CheckChanges()) {
        return false;
    }
    if (!nodeId) {
        nodeId = GetSelectedNodeId();
    }
    if (nodeId != 0) {
        var mode = 'edit';
        SetMode(mode, true);

        if (query == null) {
            query = '';
        }
        if (classId != null) {
            query += "&classid=" + classId;
        }

        if (refreshTree) {
            RefreshTree(0, nodeId);
        }

        window.PerformContentRedirect(mode, 'new', nodeId, query);
        return true;
    }
    return false;
}

// Delete item action
function DeleteItem(nodeId, refreshTree) {
    if (!CheckChanges()) {
        return false;
    }
    if (!nodeId) {
        nodeId = GetSelectedNodeId();
    }
    if (nodeId > 0) {
        if (refreshTree) {
            RefreshTree(0, nodeId);
        }

        window.PerformContentRedirect(null, 'delete', nodeId, null);
        return true;
    }
    return false;
}

// Move UP item action
function MoveUp(nodeId) {
    if (!CheckChanges()) {
        return false;
    }
    if (!nodeId) {
        nodeId = GetSelectedNodeId();
    }
    if (nodeId > 0) {
        window.ProcessRequest('moveup', nodeId);
        EnsureProperSelectedNode(nodeId);
        return true;
    }
    return false;
}

// Move DOWN item action
function MoveDown(nodeId) {
    if (!CheckChanges()) {
        return false;
    }
    if (!nodeId) {
        nodeId = GetSelectedNodeId();
    }
    if (nodeId > 0) {
        window.ProcessRequest('movedown', nodeId);
        EnsureProperSelectedNode(nodeId);
        return true;
    }
    return false;
}

//#endregion

// Maximize the content area from main menu
function FullScreen() {
    if (Minimize) {
        Minimize();
    }
}

/*#region TREE OPERATIONS*/
// Refresh node action
function RefreshTree(expandNodeId, selectNodeId) {
    if (selectNodeId == null) {
        selectNodeId = currentNodeId;
    }
    SetSelectedNodeId(selectNodeId);
    window.ProcessRequest('refresh', selectNodeId, expandNodeId);
}

function AllowClick() {
    if (window.allowClick && (window.allowClick > new Date())) {
        return false;
    }

    return true;
}

// Select node action
function SelectNode(nodeId, nodeElem, tab) {
    if (!AllowClick() || !CheckChanges()) {
        return false;
    }
    if (nodeElem == null) {
        // Current node is original
        nodeElem = currentNode;
    }

    if (!CheckChanges()) {
        return false;
    }

    if ((currentNode != null) && (nodeElem != null) && (nodeId != currentNodeId)) {
        currentNode.className = 'ContentTreeItem';
    }

    if (nodeId != null) {
        SetSelectedNodeId(nodeId);
    }
    DisplayDocument(tab);
    currentNodeId = nodeId;

    if (nodeElem != null) {
        currentNode = nodeElem;
        if (currentNode != null) {
            currentNode.className = 'ContentTreeSelectedItem';
        }
    }
}

// Maintains scroll position of the tree
function MaintainScroll(nodeId, treeId, originalScroll) {
    var elm = $cmsj('#handle_' + nodeId);
    var pnl = $cmsj('#' + treeId);
    var actionsPanel = $cmsj('.tree-actions-panel');
    var origScroll = originalScroll;
    var elmOff = elm.offset();
    var elmPos = (elmOff == null) ? 0 : elmOff.top;
    var actionsSize = 8;
    if (actionsPanel.length > 0) {
        actionsSize += actionsPanel.height();
    }
    var scroll = ((elmPos < origScroll) || (elmPos > (origScroll + pnl.height())));
    pnl.scrollTop(origScroll);
    
    if (scroll) { pnl.animate({ scrollTop: elmPos - actionsSize }, 300); };
}

// Refreshes tree and keeps current node selected
function TreeRefresh() {
    RefreshTree(currentNodeId, null);
}

//#endregion


/*#region MODES*/

// Opens search page
function OpenSearch() {
    if (!CheckChanges()) {
        return false;
    }

    window.PerformContentRedirect(null, 'search', 0, null);
    return true;
}

// Display listing
function Listing(nodeId) {
    // Check whether to refresh tree before node is selected
    var refreshTree = (currentNodeId != nodeId);
    SetMode('listing', true);
    SelectNode(nodeId);
    if (refreshTree) {
        RefreshTree(nodeId, nodeId);
    }
}

//#endregion


/*#region CONTEXT MENU ACTIONS*/

// Move item action
function MoveItem(nodeId) {
    if (nodeId != 0) {
        // Display node selection dialog in move mode
        HideContextMenu('nodeMenu');
        modalDialog(document.getElementById('hdnMoveNodeUrl').value, 'contentselectnode', '90%', '85%');
    }
}

// Copy item action
function CopyItem(nodeId) {
    if (nodeId != 0) {
        // Display node selection dialog in copy mode
        HideContextMenu('nodeMenu');
        modalDialog(document.getElementById('hdnCopyNodeUrl').value, 'contentselectnode', '90%', '85%');
    }
}

// Displays the document properties page
function Properties(nodeId, tab) {
    if (nodeId != 0) {
        SetMode('edit', true);
        RefreshTree(0, nodeId);

        window.PerformContentRedirect(null, 'content', nodeId, '&tabname=' + tab);
    }
}

// Ensures that proper node is displayed after content or context menu action
function EnsureProperSelectedNode(nodeId) {
    if (nodeId != GetSelectedNodeId()) {
        SelectNode(nodeId);
    }
}

// Performs the sort node action
function SortAlphaAsc(nodeId) {
    if (nodeId > 0) {
        window.ProcessRequest('sortalphaasc', nodeId);
        EnsureProperSelectedNode(nodeId);
    }
}

// Performs the sort node action
function SortAlphaDesc(nodeId) {
    if (nodeId > 0) {
        window.ProcessRequest('sortalphadesc', nodeId);
        EnsureProperSelectedNode(nodeId);
    }
}

// Performs the sort node action
function SortDateAsc(nodeId) {
    if (nodeId > 0) {
        window.ProcessRequest('sortdateasc', nodeId);
        EnsureProperSelectedNode(nodeId);
    }
}

// Performs the sort node action
function SortDateDesc(nodeId) {
    if (nodeId > 0) {
        window.ProcessRequest('sortdatedesc', nodeId);
        EnsureProperSelectedNode(nodeId);
    }
}

// Moves the node to the top
function MoveTop(nodeId) {
    if (nodeId > 0) {
        window.ProcessRequest('movetop', nodeId);
        EnsureProperSelectedNode(nodeId);
    }
}

// Moves the node to the top
function MoveBottom(nodeId) {
    if (nodeId > 0) {
        window.ProcessRequest('movebottom', nodeId);
        EnsureProperSelectedNode(nodeId);
    }
}

//#endregion


/*#region LANGUAGE SELECTION*/

// Changes the language
function ChangeLanguage(language) {
    if (!CheckChanges()) {
        return false;
    }
    SetSelectedCulture(language);
    cmsrequire(['CMS/EventHub', 'CMS.Builder/Constants'], function (eventHub, constants) {
        eventHub.publish(constants.ADMIN_FRAME_REQUEST_AUTHENTICATION_EVENT_NAME, { culture: language });
    });

    window.ProcessRequest('setculture', GetSelectedNodeId(), language);
    return true;
}

//#endregion


/*#region DRAG & DROP HANDLING*/

function DragOperation(nodeId, targetNodeId, operation) {
    window.PerformContentRedirect(null, 'drag', nodeId, '&action=' + operation + '&targetnodeid=' + targetNodeId);
}

function CancelDragOperation() {
    if (frames['contentview'].CancelDragOperation) {
        frames['contentview'].CancelDragOperation();
    }
}

//#endregion


/*#region URL RETRIEVING*/

String.prototype.startsWith = function (str) { return (this.match('^' + str) == str); };

var URL_PREFIX = '##URL##';

function CopyMoveItem(data) {
    if (data != null && data !== '') {
        var urls = data.split('##SEP##');
        modalDialog(urls[0], 'contentselectnode', '90%', '85%');
    }
}

function ViewContent(data) {
    if (data != null && data !== '') {
        var urls = data.split('##SEP##');
        frames['contentview'].location.href = urls[0];
        if (urls.length > 1) {
            SetLiveSiteURL(urls[1]);
        }
    }
}

function SetLiveSiteURL(url) {
    if ((parent != this) && parent.SetLiveSiteURL) {
        parent.SetLiveSiteURL(url);
    }
}

function Error(jqXHR, textStatus, errorThrown) {
    if (jqXHR.status == 500) {
        if ((errorThrown == null) || (errorThrown == '')) {
            errorThrown = 'Internal Server Error';
        }
        if (confirm('Error occurred: \'' + errorThrown + '\'. Please check the event log. Do you want to refresh the UI?')) {
            document.location.href = document.location.href;
        }
    }
}

function VerifyData(data) {
    if (data.startsWith(URL_PREFIX)) {
        return data.substring(URL_PREFIX.length);
    }
    else if (data.indexOf('__VIEWSTATE') != -1) {
        // Most probably user is not logged in anymore
        frames['contentview'].location.href = frames['contentview'].location.href;
        data = null;
    }

    return data;
}

function PerformRedirect(mode, action, nodeId, culture, query, originalUrl, transformToCompare, successCallback, errorCallback) {
    var SEP = '##SEP##';

    if (mode == null) {
        mode = GetSelectedMode();
    }

    if (!culture) {
        culture = GetSelectedCulture();
    }

    var arg = URL_PREFIX + (mode || '') + SEP + (action || '') + SEP + nodeId + SEP + (culture || '') + SEP + (query || '') + SEP + (originalUrl || '') + SEP + (transformToCompare || 'false') + SEP + 'true';
    arg = escape(arg);
    $cmsj.ajax({
        cache: false,
        type: 'GET',
        data: 'urlparams=' + arg,
        context: document.body,
        success: successCallback,
        error: errorCallback || Error,
        dataType: 'text',
        dataFilter: VerifyData
    });
}


function PerformDialogAction(action, nodeId) {
    var queryString = null;

    if (nodeId > 0) {
        if (action === 'LinkDoc') {
            queryString = 'content_sites=OnlyCurrentSite';
        } 

        PerformRedirect('edit', action, nodeId, null, queryString, null, false, CopyMoveItem);        
    }
}


function PerformContentRedirect(mode, action, nodeId, query, culture) {
  PerformRedirect(mode, action, nodeId, culture, query, null, false, ViewContent);
}

function CopyRef(arg) {
    PerformDialogAction('Copy', arg);
}

function MoveRef(arg) {
    PerformDialogAction('Move', arg);
}

function LinkRef(arg) {
    PerformDialogAction('LinkDoc', arg);
}

//#endregion