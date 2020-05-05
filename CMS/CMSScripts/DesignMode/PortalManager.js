var webPartLocation = new Array();
var webPartZoneLocation = new Array();
var placeholderProperties = new Array();

var bordersToDeactivate = new Array();
var borderDeactivationEnabled = true;
var refreshPageLocationUrl = null;

var copy_allowedType = null;
var copy_changeActiveItem = null;

window.useDraggedClass = true;

var pmCopyInProgress = false;
var pmPasteInProgress = false;
var pmPasteEnabled = false;

function webPartProperties(zoneId, webPartId, nodeAliasPath, instanceGuid, templateId, clientId, variantId, clipBoardItem, key) {
    this.clientId = clientId;
    this.zoneId = zoneId;
    this.webPartId = webPartId;
    this.nodeAliasPath = nodeAliasPath;
    this.instanceGuid = instanceGuid;
    this.templateId = templateId;
    this.variantId = variantId;
    this.containsClipBoardItem = clipBoardItem;
    this.clipBoardKey = key;
}


function zoneProperties(zoneId, nodeAliasPath, templateId, zoneType, layoutZone, clipBoardItem, key) {
    this.zoneId = zoneId;
    this.nodeAliasPath = nodeAliasPath;
    this.templateId = templateId;
    this.zoneType = zoneType;
    this.layoutZone = layoutZone;
    this.containsClipBoardItem = clipBoardItem;
    this.clipBoardKey = key;
}


// General
function Get(id) {
    return document.getElementById(id);
}

function SetVal(id, val) {
    var elem = Get(id); if (elem != null) { elem.value = (val != null ? val : ''); }
}

// Set the page state which identifies whether the document content has been modified of not.
function SetContentChanged(modified) {
    if (typeof modified == "undefined") {
        modified = true;
    }

    if (window.CMSContentManager) {
        CMSContentManager.changed(modified);
    }
}

// Set the page state which identifies whether the page controls have been modified of not (added widget, moved web part...).
function SetPageChanged() {
    Get('pageChanged').value = 1;
}


function WP_GetMenuContext(el) {
    while (el != null) {
        if (el.className.indexOf("FreeLayout") >= 0) {
            return { hideClass: "NormalLayoutMenu", showClass: "AnyMenu" };
        }
        else if (el.className.indexOf("ModifyNotAllowed") >= 0) {
            return { hideClass: "MultipleMenu", showClass: "AnyMenu" };
        }
        else if (el.className.indexOf("WebPartZone") >= 0) {
            break;
        }

        el = el.parentNode;
    }

    return { hideClass: "FreeLayoutMenu", showClass: "AnyMenu" };
}

function WP_ContextMenu(wp, param) {
    var p = wp.parentNode.parentNode.parentNode.parentNode;
    var ctx = WP_GetMenuContext(p);
    ContextMenu('webPartMenu', p, param, true, null, null, null, null, ctx);
    return false;
}

function WG_ContextMenu(wg, param) {
    var p = wg.parentNode.parentNode.parentNode.parentNode;
    ContextMenu('widgetMenu', p, param, true);
    return false;
}

function getZoneVariantId(zoneId) {
    return ((typeof (window.GetCurrentZoneVariantId) == 'function') ? GetCurrentZoneVariantId(zoneId) : 0);
}

// Web parts
function MoveWebPartAsync(properties, targetZoneId, targetPosition, elem) {
    if (elem.keepPosition) {
        targetPosition = '';
    }
    var param = 'MoveWebPartAsync\n' + properties.zoneId + '\n' + properties.webPartId + '\n' + properties.nodeAliasPath + '\n' + properties.instanceGuid + '\n' + targetZoneId + '\n' + targetPosition + '\n' + getZoneVariantId(properties.zoneId) + '\n' + getZoneVariantId(targetZoneId);
    if (elem.flowLayout) {
        param += '\n' + elem.flowX + '\n' + elem.flowY;
    }
    SetContentChanged();
    SetPageChanged();
    PM_Callback(param, function (rvalue, context) {
        if ((rvalue != null) && (rvalue != '')) {
            SetContentChanged();
            var values = rvalue.split('<#>');
            switch (values[0]) {
                case 'U':
                    window.top.location.replace(window.top.location.href);
                    break;
                case 'R':
                    RefreshPage(true);
                    break;
                case 'E':
                    alert(values[1]);
                    break;
                case 'ER':
                    alert(values[1]);
                    RefreshPage();
                    break;
                case 'UPDATE_IDS':
                    if (typeof (UpdateWidgetInputElementIdentifiers) == 'function') {
                        UpdateWidgetInputElementIdentifiers(values[1], values[2]);
                    }
                    break;
            }
        }

        // Set new zoneID if zone was changed
        if (properties.zoneId != targetZoneId) {
            properties.zoneId = targetZoneId;

            if (typeof (itemCodesAssociativeArray) != "undefined") {
                var itemInfo = itemCodesAssociativeArray["Variant_WP_" + properties.instanceGuid.replace(/-/g, '')];
                if (itemInfo != null) {
                    itemInfo[9] = targetZoneId;
                }
            }
        }

        context.className = context.originalClassName;
    }, elem);
}

function GetActualZoneId(clientId) {
    var obj = webPartLocation[clientId + '_container'];
    if (obj != null) {
        return obj.zoneId;
    }

    return null;
}

function SetWebPartProperty(clientId, propertyName, value, line) {
    var loc = webPartLocation[clientId + '_container'];
    value = value.replace(/[\r\n]+/g, '[\\n]');
    var param = 'SetWebPartProperty\n' + loc.zoneId + '\n' + loc.webPartId + '\n' + loc.nodeAliasPath + '\n' + loc.instanceGuid + '\n' + loc.variantId + '\n' + propertyName + '\n' + value + '\n' + line;
    SetContentChanged();
    SetPageChanged();
    PM_Callback(param, OnReceiveScript);
}

function AddToWebPartProperty(clientId, propertyName, value, refresh) {
    var loc = webPartLocation[clientId + '_container'];
    var param = 'AddToWebPartProperty\n' + loc.zoneId + '\n' + loc.webPartId + '\n' + loc.nodeAliasPath + '\n' + loc.instanceGuid + '\n' + loc.variantId + '\n' + propertyName + '\n' + value + '\n' + refresh;
    SetContentChanged();
    SetPageChanged();
    PM_Callback(param, OnReceiveScript);
}

function AddWebPart(webPartId) {
    setWebPart(webPartId);
    PM_Postback('AddWebPart');
}

function NewWebPart(wz) {
    setZone(wz.zoneId);
    setAliasPath(wz.nodeAliasPath);
    setTemplate(wz.templateId);
    isLayoutZone = !!wz.layoutZone;
    setIsLayoutZone(isLayoutZone);

    var url = webPartsPath + 'WebPartSelector.aspx';
    if (window.isUI) {
        url += '?isui=true';
    }

    modalDialog(url, 'selectwebpart', '90%', '85%');
}

function AddWebPartWithoutDialog(webPartId, webPartZoneId, isLayoutZone, position) {
    setWebPart(webPartId);
    setTargetZone(webPartZoneId);
    setTargetPosition(position);
    setIsLayoutZone(isLayoutZone);
    PM_Postback('AddWebPartWithoutDialog');
}

function ConfigureWebPartZone(zone) {
    var variantId = 0;
    if (typeof (GetCurrentVariantId) == 'function') {
        variantId = GetCurrentVariantId('Variant_Zone_' + zone.zoneId);
    }

    var type = '';
    if (typeof (GetCurrentObjectVariantMode) == 'function') {
        type = GetCurrentObjectVariantMode('Variant_Zone_' + zone.zoneId);
        type = (type != '') ? '&variantmode=' + type : '';
    }

    modalDialog(webPartsPath + 'WebPartZoneProperties.aspx?aliaspath=' + zone.nodeAliasPath + '&culture=' + getCulture() + '&zoneid=' + zone.zoneId + '&templateid=' + zone.templateId + '&layoutzone=' + zone.layoutZone + type + ((variantId > 0) ? '&variantid=' + variantId : ''), 'configurewebpartzone', 950, '85%');
}

function RemoveAllWebParts(zone) {
    if (confirm(confirmRemoveAll)) {
        setZone(zone.zoneId);
        setAliasPath(zone.nodeAliasPath);
        PM_Postback('RemoveAllWebParts');
    }
}

function MoveAllWebParts(zone, targetZoneId) {
    setZone(zone.zoneId); setAliasPath(zone.nodeAliasPath); setTargetZone(targetZoneId);
    PM_Postback('MoveAllWebParts');
}

function OnSelectWebPart(webPartId, skipDialog) {
    if (skipDialog) {
        AddWebPartWithoutDialog(webPartId, getZone(), getIsLayoutZone(), -1);
    }
    else {
        modalDialog(webPartsPath + 'WebPartProperties.aspx?aliaspath=' + getAliasPath() + '&culture=' + getCulture() + '&templateid=' + getTemplate() + '&zoneid=' + getZone() + '&zonevariantid=' + getZoneVariantId(getZone()) + '&webpartid=' + webPartId + '&layoutzone=' + getIsLayoutZone() + '&isnew=1&', 'configurewebpart', 950, '85%');
    }
}

function MoveWebPart(webPart, targetZoneId, targetPosition) {
    setZone(webPart.zoneId);
    setWebPart(webPart.webPartId);
    setAliasPath(webPart.nodeAliasPath);
    setGuid(webPart.instanceGuid);
    setTargetZone(targetZoneId);
    setTargetPosition(targetPosition);

    PM_Postback('MoveWebPart');
}

function trim(s) {
    if (s == null) return s;
    return s.replace(/^\s+|\s+$/g, "");
}

function MoveElemDown(el, bottom) {
    if (bottom) {
        var p = el.parentNode;
        p.removeChild(el);
        p.appendChild(el);
        return;
    }

    var next = el.nextSibling;
    while (next != null) {
        if (trim(next.className) == trim(el.className)) {
            var par = el.parentNode;
            next = next.nextSibling;
            par.removeChild(el);
            if (next != null) par.insertBefore(el, next);
            else par.appendChild(par);
            break;
        }
        next = next.nextSibling;
    }
}

function MoveWebPartUp(webPart, top) {
    var param = '\n' + webPart.zoneId + '\n' + webPart.webPartId + '\n' + webPart.nodeAliasPath + '\n' + webPart.instanceGuid;
    var op = (top ? 'MoveWebPartTop' : 'MoveWebPartUp');

    var el = Get(webPart.clientId + "_container");
    if (el.style.position == 'relative') {
        MoveElemDown(el, top);
        op = (top ? 'MoveWebPartBottom' : 'MoveWebPartDown');
    }
    else {
        MoveElemUp(el, top);
    }

    PM_Callback(op + param, OnReceiveScript);
}

function MoveElemUp(el, top) {
    if (top) {
        var p = el.parentNode;
        p.removeChild(el);
        var first = p.firstChild;
        if ((first != null) && (first != el)) {
            p.insertBefore(el, first);
        }
        else {
            p.appendChild(el);
        }
        return;
    }

    var prev = el.previousSibling;
    while (prev != null) {
        if (trim(prev.className) == trim(el.className)) {
            var par = el.parentNode;
            par.removeChild(el);
            par.insertBefore(el, prev);
            break;
        }
        prev = prev.previousSibling;
    }
}

function MoveWebPartDown(webPart, bottom) {
    var param = '\n' + webPart.zoneId + '\n' + webPart.webPartId + '\n' + webPart.nodeAliasPath + '\n' + webPart.instanceGuid;
    var op = (bottom ? 'MoveWebPartBottom' : 'MoveWebPartDown');

    var el = Get(webPart.clientId + "_container");
    if (el.style.position == 'relative') {
        MoveElemUp(el, bottom);
        op = (bottom ? 'MoveWebPartTop' : 'MoveWebPartUp');
    }
    else {
        MoveElemDown(el, bottom);
    }

    PM_Callback(op + param, OnReceiveScript);
}

function CloneWebPart(wp) {
    setZone(wp.zoneId);
    setWebPart(wp.webPartId);
    setAliasPath(wp.nodeAliasPath);
    setGuid(wp.instanceGuid);

    PM_Postback('CloneWebPart');
}

function CopyWebPart(webPart, menuId) {

    if (!pmCopyInProgress) {
        pmCopyInProgress = true;

        // Display loader
        if (window.Loader) {
            window.Loader.show();
        }

        var wpId = (typeof (webPart.webPartId) == 'undefined') ? '' : webPart.webPartId;

        // Set parameters
        var param = 'CopyWebPart\n' + webPart.zoneId + '\n' + wpId + '\n' + webPart.nodeAliasPath + '\n' + webPart.instanceGuid + '\n' + getZoneVariantId(webPart.zoneId);

        // Do call back
        PM_Callback(param, function (rvalue, context) {

            pmCopyInProgress = false;

            // Hide loader
            if (window.Loader) {
                window.Loader.hide();
            }

            if (rvalue == 'ok') {
                // Close menu
                CM_Close(menuId);

                // Enable paste item
                if (menuId) {
                    PM_EnablePasteValue(menuId);
                }
            }
        });
    }
}

function PasteWebPart(webPart) {
    if (!pmPasteInProgress && pmPasteEnabled) {
        pmPasteInProgress = true;

        // Display loader
        if (window.Loader) {
            window.Loader.show();
        }

        var param = 'PasteWebPart\n' + webPart.zoneId + '\n' + webPart.webPartId + '\n' + webPart.nodeAliasPath + '\n' + webPart.instanceGuid + '\n' + getZoneVariantId(webPart.zoneId) + '\n' + webPart.layoutZone + '\n' + webPart.zoneType;
        PM_Callback(param, function (rvalue, context) {
            pmPasteInProgress = false;

            if (rvalue == 'ok') {
                // Refresh page hides loader automatically.
                RefreshPage(true);
            }
            else {
                if (window.Loader) {
                    window.Loader.hide();
                }
            }
        });
    }
}


function RemoveWebPart(webPart) {
    if (confirm(confirmRemove)) {
        setZone(webPart.zoneId); setWebPart(webPart.webPartId); setAliasPath(webPart.nodeAliasPath); setGuid(webPart.instanceGuid);
        PM_Postback('RemoveWebPart');
    }
}

function PM_EnsurePasteHandler(menuId, pasteId) {
    CM_RegisterMenuHandler(menuId, PM_PasteItemVisibility, pasteId);
}


function PM_PasteItemVisibility(menuId, definition, pasteId) {
    // Get paste element
    var elem = document.getElementById(pasteId);
    // Indicates whether paste should be active or not
    var isActive = null;

    if (elem != null) {
        // Try enable current copy item
        if (copy_changeActiveItem) {
            copy_allowedType = definition.clipBoardKey;
            copy_changeActiveItem = false;
        }
        else if (copy_allowedType != null) {
            if (definition.clipBoardKey == copy_allowedType) {
                isActive = true;
            }
        }
            // Get state from original definition
        else {
            isActive = definition.containsClipBoardItem;
        }

        if (isActive) {
            elem.className = 'Item';
            pmPasteEnabled = true;
        }
        else {
            elem.className = 'Item ItemDisabled';
            pmPasteEnabled = false;
        }
    }
}

var PM_PasteClickHandler = function () {
    alert("I'm in");
    return false;
}

function PM_EnablePasteValue(menuId) {
    definition = GetContextMenuParameter(menuId);
    copy_changeActiveItem = true;
    CM_InvokeHandlers(menuId, definition);
}


function OnAddWebPartVariant(query) {
    modalDialog(webPartsPath + 'WebPartProperties.aspx' + query + '&isnewvariant=true', 'configurewebpart', 950, '85%');
}

function OnAddWebPartZoneVariant(query) {
    modalDialog(webPartsPath + 'WebPartZoneProperties.aspx' + query + '&isnewvariant=true', 'configurewebpartzone', 950, '85%');
}

function ConfigureWebPart(properties) {

    var url = webPartsPath + 'WebPartProperties.aspx?aliaspath=' + properties.nodeAliasPath + '&culture=' + getCulture() + '&zoneid=' + escape(properties.zoneId) + '&webpartid=' + escape(properties.webPartId) + '&instanceguid=' + escape(properties.instanceGuid) + '&templateid=' + escape(properties.templateId);
    var variantId = 0;
    var zoneVariantId = 0;
    if (typeof (GetCurrentVariantId) == 'function') {
        variantId = GetCurrentVariantId('Variant_WP_' + escape(properties.instanceGuid).replace(/-/g, ''));
        zoneVariantId = GetCurrentVariantId('Variant_Zone_' + properties.zoneId);
    }

    if (variantId > 0) {
        url += '&variantid=' + variantId;
    }
    if (zoneVariantId > 0) {
        url += '&zonevariantid=' + zoneVariantId;
    }

    var step = GetCurrentStepIndexFor(escape(properties.clientId));
    if (step >= 0) {
        url += '&step=' + step;
    }

    modalDialog(url, 'configurewebpart', 950, '85%');
}

function GetCurrentStepIndexFor(clientId) {
    if (clientId != null) {
        var el = Get(clientId + "_container");
        while (el != null) {
            if (el.currentStep != null) {
                return el.currentStep;
            }

            el = el.parentNode;
        }
    }

    return -1;
}

// Template
function CloneTemplate(aliasPath) {
    setAliasPath(aliasPath);
    PM_Postback('CloneTemplate');
}

// Widgets
function NewWidget(zone) {
    var path = encodeURIComponent(zone.nodeAliasPath).replace(/%2F/g, "%2f");
    setZone(zone.zoneId);
    setZoneType(zone.zoneType);
    setAliasPath(path);
    setTemplate(zone.templateId);

    isLayoutZone = !!zone.layoutZone;
    setIsLayoutZone(isLayoutZone);
    modalDialog(widgetSelectorUrl + '?templateid=' + zone.templateId + '&zoneid=' + escape(zone.zoneId), 'selectwidget', '90%', '85%');
}

function AddWidgetWithoutDialog(widgetId, widgetZoneId, isLayoutZone) {
    SetContentChanged();
    SetPageChanged();

    var param = 'AddWidgetWithoutDialog\n' + widgetId + '\n' + widgetZoneId + '\n' + isLayoutZone + '\n' + getZoneType();
    PM_Callback(param, function (rvalue, context) {
        RefreshPage(true);
    });
}

function OnSelectWidget(widgetId, skipDialog) {
    if (skipDialog) {
        AddWidgetWithoutDialog(widgetId, getZone(), getIsLayoutZone());
    }
    else {
        modalDialog(widgetPropertiesUrl + '?aliaspath=' + getAliasPath() + '&culture=' + getCulture() + '&templateid=' + getTemplate() + '&zoneid=' + getZone() + '&zonetype=' + getZoneType() + '&widgetid=' + widgetId + '&layoutzone=' + getIsLayoutZone() + '&isnew=1' + getDashBoardParameter(), 'configurewidget', 950, '85%');
    }
}

function ConfigureWidget(widget) {
    var variantId = 0;
    if (typeof (GetCurrentVariantId) == 'function') {
        variantId = GetCurrentVariantId('Variant_WP_' + widget.instanceGuid.replace(/-/g, ''));
    }

    // As aliasPath starts with '/', prevent additional encoding if it is already encoded
    var aliasPath = widget.nodeAliasPath;
    if ((aliasPath.length > 0) && (aliasPath[0] != '%')) {
        aliasPath = encodeURIComponent(aliasPath);
    }

    var url = widgetPropertiesUrl + '?aliaspath=' + aliasPath + '&culture=' + getCulture() + '&templateid=' + widget.templateId + '&zoneid=' + widget.zoneId + '&widgetid=' + widget.webPartId + '&instanceguid=' + widget.instanceGuid;

    if (variantId > 0) {
        url += '&variantid=' + variantId;
    }

    url += getDashBoardParameter();

    var step = GetCurrentStepIndexFor(widget.clientId);
    if (step >= 0) {
        url += '&step=' + step;
    }

    modalDialog(url, 'configurewidget', 950, '85%');
}

function RemoveWidget(widget) {
    if (confirm(confirmRemoveWidget)) {

        var param = "RemoveWebPart\n" + widget.zoneId + "\n" + widget.webPartId + "\n" + widget.nodeAliasPath + "\n" + widget.instanceGuid;

        SetContentChanged();
        PM_Callback(param, function (rvalue, context) {
            RefreshPage(true);
        });
    }
}

function RemoveAllWidgets(zone) {
    if (confirm(confirmRemoveAllWidgets)) {

        var param = "RemoveAllWebParts\n" + zone.zoneId + "\n" + zone.nodeAliasPath;

        SetContentChanged();
        PM_Callback(param, function (rvalue, context) {
            RefreshPage(true);
        });
    }
}

function ToggleMinimizeWidget(elem, id, widgetZoneId, widgetId, aliasPath, instanceGuid) {
    var param = '\n' + widgetZoneId + '\n' + widgetId + '\n' + aliasPath + '\n' + instanceGuid;
    containerElem = Get(id + '_container');
    // Inner container
    containerElem = containerElem.firstElementChild;
    if (!/ MinimizedWidget/.test(containerElem.className)) {
        param = 'MinimizeWidget' + param;
        containerElem.className += ' MinimizedWidget';
        elem.className = elem.className.replace(/minimize/i, 'maximize');
        elem.title = maximizeWidget;
        PM_Callback(param, WidgetStateChanged, containerElem);
    } else {
        param = 'MaximizeWidget' + param;
        containerElem.className = containerElem.className.replace(/ MinimizedWidget/, '');
        elem.className = elem.className.replace(/maximize/i, 'minimize');
        elem.title = minimizeWidget;
        PM_Callback(param, WidgetStateChanged, containerElem);
    }
}

function WidgetStateChanged(rvalue, context) {
}

function CannotModifyUserWidgets() {
    alert(cannotModifyWidgets);
}

function OnBeforeDropWebPart(sender, e) {
    var container = e.get_container();
    var item = e.get_droppedItem();
    var position = e.get_position();

    if (typeof (BeforeDropWebPart) == 'function') {
        BeforeDropWebPart(container, item, position);
    }
}

function OnDropWebPart(sender, e) {
    var container = e.get_container();
    var item = e.get_droppedItem();
    var position = e.get_position();

    item.originalClassName = item.className;

    if (window.useDraggedClass) {
        item.originalClassName = item.originalClassName.replace("DraggedWebPart", "");
    }

    item.className = item.className + ' WebPartUpdating';

    var uid = item.getAttribute('id');
    var loc = webPartLocation[uid];

    var zid = container.getAttribute('id');
    var targetzoneid = webPartZoneLocation[zid].zoneId;

    if ((typeof (loc) != 'undefined') && (loc != null)) {
        DeactivateBorder(uid.replace('_container', ''), 0);

        MoveWebPartAsync(loc, targetzoneid, position, item);
    }
    else {
        var pnlWebPart = container.getAttribute('id');
        var zone = webPartZoneLocation[pnlWebPart];
        var isLayoutZone = !!zone.layoutZone;
        var webPartId = item.getAttribute('data-webpartid');
        var zoneVariantId = getZoneVariantId(zone.zoneId);

        if (item.flowLayout) {
            item.flowX += document.documentElement.scrollLeft;
            item.flowY += document.documentElement.scrollTop;

            position += '|' + item.flowX + '|' + item.flowY;
        }

        if (item.getAttribute('data-skipdialog') != 1) {
            modalDialog(webPartsPath + 'WebPartProperties.aspx?aliaspath=' + zone.nodeAliasPath + '&culture=' + getCulture() + '&templateid=' + zone.templateId + '&zoneid=' + zone.zoneId + ((zoneVariantId > 0) ? '&zonevariantid=' + zoneVariantId : '') + '&webpartid=' + webPartId + '&layoutzone=' + isLayoutZone + '&isnew=1&position=' + position, 'configurewebpart', 950, '85%');
        }
        else {
            AddWebPartWithoutDialog(webPartId, zone.zoneId, isLayoutZone, position);
        }
    }
}

function OnReceiveScript(rvalue, context) {
    if ((rvalue != null) && (rvalue != '')) {
        setTimeout(rvalue, 1);
    }
}

function UpdateRefreshPageUrlParam(param, value) {
    if ((typeof (refreshPageLocationUrl) == 'undefined') || (refreshPageLocationUrl == null)) {
        refreshPageLocationUrl = document.location + '';
    }

    refreshPageLocationUrl = $cmsj.param.querystring(refreshPageLocationUrl, param + "=" + value);
}

function RefreshPage(usePostback) {
    if (usePostback) {
        SetContentChanged();
        PM_Postback('Refresh');
    }
    else {
        var url = document.location + '';

        if (window.CMSContentManager) {
            // Store the data object in the refresh url (to be able to populate the object after refresh)
            url = CMSContentManager.storeContentChangedInUrl(url);
            // Do not handle confirm changes. In this case all data should be saved
            CMSContentManager.confirmChanges = false;
        }

        if ((typeof (refreshPageLocationUrl) != 'undefined') && (refreshPageLocationUrl != null)) {
            url = refreshPageLocationUrl;
        }
        document.location.replace(url.replace('safemode=1', 'safemode=0'));
    }
}

function RefreshWOpener(window) {
    if (window.refreshPageOnClose) {
        RefreshPage(usePostbackRefresh);
    }
}

function ActivateBorder(id, elem) {
    ClearBorderDeactivation(id);

    var e = Get(id + '_border');
    if (e != null) {
        if ((e.className.indexOf('WebPartZoneBorder') != -1) || (e.className.indexOf('WebPartBorder') != -1)) {
            e.className = ' ' + e.className + ' ';
            e.className = e.className.replace(' WebPartZoneBorder ', ' WebPartZoneBorderActive ');
            e.className = e.className.replace(' WebPartBorder ', ' WebPartBorderActive ');
            // Trim end
            e.className = e.className.replace(/^\s+|\s+$/g, '');
        }
    }
}

function DeactivateBorder(id, time) {
    if (borderDeactivationEnabled) {
        if (time <= 0) {
            bordersToDeactivate[id] = null;
            var e = Get(id + '_border');
            if (e != null) {
                if ((e.className.indexOf('WebPartZoneBorder') != -1) || (e.className.indexOf('WebPartBorder') != -1)) {
                    e.className = e.className.replace('WebPartZoneBorderActive', ' WebPartZoneBorder ');
                    e.className = e.className.replace('WebPartBorderActive', ' WebPartBorder ');

                    // Trim end
                    e.className = e.className.replace(/^\s+|\s+$/g, '');
                }
            }
        } else {
            ClearBorderDeactivation(id);
            bordersToDeactivate[id] = setTimeout("DeactivateBorder('" + id + "', 0)", time);
        }
    }
}

function ClearBorderDeactivation(id) {
    var toDeactivate = bordersToDeactivate[id];
    if (toDeactivate != null) {
        bordersToDeactivate[id] = null;
        clearTimeout(toDeactivate);
    }
}