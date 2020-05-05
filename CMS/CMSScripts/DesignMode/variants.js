//////////////////////
// Global variables
//////////////////////

var webPartMVTVariantContextMenuId = null;
var zoneMVTVariantContextMenuId = null;
var webPartCPVariantContextMenuId = null;
var zoneCPVariantContextMenuId = null;
var cpVariantSliderPositionElem = null;
var variantSliderChanged = false;
var variantSliderIsRTL = false;
var currentContextMenuId = null;

// Zone IDs with variants denied (i.e. - a zone which contains the web part "Zones with effect" - does not work with MVT)
var zonesWithVariantsDenied = new Array();

var itemCodesAssociativeArray = new Object(); // item = webpart/zone/widget
// itemCode[0]: array of variantIDs ([0 - original, 1.. - variantIDs] )
// itemCode[1]: array of variant DIVs - display: block/none ([0 - original, 1.. - variantIDs] )
// itemCode[2]: array of variant control IDs - ([0 - original, 1.. - variant control IDs] )
// itemCode[3]: actual slider position (0 - original, 1.. - variant)
// itemCode[4]: total variants (0 - just original, 1.. - number of variants)
// itemCode[5]: variant slider id
// itemCode[6]: variant slider element - this is the draggable slider
// itemCode[7]: variant slider - hidden slider position id
// itemCode[8]: web part/zone title id
// itemCode[9]: zone id
// itemCode[10]: web part instance guid
// itemCode[11]: 'mvt'/'personalization' - indicates whether the variant is a MVT or ContentPersonalization variant
// itemCode[12]: slider client id

// Temporary arrays
var itemCodes = new Array();
var itemIDs = new Array();
var itemControlIDs = new Array();
var divIDs = new Array();


/////////////////////////
// Window.OnLoad/OnUnload event
/////////////////////////

var existingOnLoad = window.onload;
window.onload = function () {
    if (typeof (existingOnLoad) == "function") { existingOnLoad(); }
    ChangeSliderImageCss();
    if (typeof (SetCombinationVariants) == "function") { SetCombinationVariants(); }
    if (typeof (InitUpdateProgress) == "function") { InitUpdateProgress(); }
    SetSelectedVariants();
};

$cmsj(window).on('beforeunload', function () {
    if (variantSliderChanged && (typeof (SaveSlidersConfiguration) == "function")) {
        // Save the sliders' positions into the cookie
        SaveSlidersConfiguration();
    }
});

////////////////////////////////
// General support functions
////////////////////////////////

function ArrayIndexOf(arr, obj) {
    for (var i = 0; i < arr.length; i++) {
        if (arr[i] == obj) {
            return i;
        }
    }
    return -1;
}

function CancelEventPropagation(e) {
    e = e || window.event;
    e.cancelBubble = true;
    if (e.stopPropagation) {
        e.stopPropagation();
    }
}

//////////////////////////////
// CMS variants functions
//////////////////////////////

// Fired when the slider changed/moved
function OnSliderChanged(evt, hdnId, totalValue, step) {
    var hdnObj = document.getElementById(hdnId);
    if (hdnObj != null) {
        var newValue = parseInt(hdnObj.value) + step;
        if ((newValue > 0) && (newValue <= totalValue)) {
            hdnObj.value = newValue;
            $cmsj(hdnObj).change();
        }
    }

    // Set the flag that indicates that the slider position has been changed
    variantSliderChanged = true;
}

// Change the label in the slider indication which variant is currently displayed (i.e.: 2/5 -> 3/5)
function OnHiddenChanged(hdnEl, lblPartEl, itemCode, sliderExtenderId) {

    // Move the slider bar
    var sliderExtender = $find(sliderExtenderId);
    var newPosition = hdnEl.value;
    var variant = parseInt(newPosition);
    sliderExtender.set_Value(newPosition);
    ChangeVariant(itemCode);

    // Reverse position index when in RTL
    if (variantSliderIsRTL) {
        var itemInfo = itemCodesAssociativeArray[itemCode];
        variant = itemInfo[4] - (variant - 1);
    }

    if (variant > 0) {
        lblPartEl.innerHTML = variant;
    }

    // Set the flag that indicates that the slider position has been changed
    variantSliderChanged = true;
}

// Updates the variation slider and displays the current variant
function ChangeVariant(itemCode) {
    var itemInfo = itemCodesAssociativeArray[itemCode];
    if (itemInfo != null) {
        // Get position info
        var sliderPosition = itemInfo[3];
        var newPosition = document.getElementById(itemInfo[7]).value;

        // Reverse position index when in RTL
        if (variantSliderIsRTL) {
            newPosition = itemInfo[4] - (newPosition - 1);
        }

        // Change the title
        var titleEl = document.getElementById(itemInfo[8]);
        if (titleEl != null) {
            titleEl.innerHTML = itemInfo[2][newPosition - 1];
        }

        // Check if the new variant object exists (the object does not exist when saving a new variant)
        if ((itemInfo[1].length >= sliderPosition) && (itemInfo[1].length >= newPosition)) {
            // Show/Hide variant DIVs
            var actualVariantObj = document.getElementById(itemInfo[1][sliderPosition - 1]);
            var newVariantObj = document.getElementById(itemInfo[1][newPosition - 1]);

            if ((actualVariantObj != null) && (newVariantObj != null)) {
                actualVariantObj.style.display = 'none';
                newVariantObj.style.display = 'block';

                // Toggle action buttons
                ToggleActionButtons(itemInfo[5], (newPosition > 1));

                // Save the new position
                itemInfo[3] = newPosition;

                if (itemInfo[11] == "mvt") {
                    // MVT
                    if (window.UpdateCombinationPanel) {
                        UpdateCombinationPanel();
                    }
                }
                else {
                    // Content personalization
                    // Update the variant positions string with the new value
                    UpdateVariantPosition(itemCode, itemInfo[0][newPosition - 1]);
                }

                // Relocate the slider
                var newLoc = document.getElementById(newVariantObj.id + "_slider");
                if (newLoc != null) {
                    var slider = document.getElementById(itemInfo[12]);
                    slider.parentNode.removeChild(slider);

                    newLoc.appendChild(slider);
                }
            }
        }
    }
}

// Enable/disable slider action buttons
function ToggleActionButtons(variantSliderId, enabled) {
    var sliderObj = document.getElementById(variantSliderId);
    var inputElems = sliderObj.getElementsByTagName("i");
    for (var i = 0; i < inputElems.length; i++) {
        if (inputElems[i].className.indexOf('SliderBtnEnabled') != -1) {
            inputElems[i].style.display = (enabled) ? 'block' : 'none';
        } else if (inputElems[i].className.indexOf('SliderBtnDisabled') != -1) {
            inputElems[i].style.display = (!enabled) ? 'block' : 'none';
        }
    }
}

// Prevents the slider bar before moving the whole web part when the bar is dragged.
function ChangeSliderImageCss() {
    for (var i in itemCodesAssociativeArray) {
        var itemInfo = itemCodesAssociativeArray[i];
        var sliderImage = document.getElementById(itemInfo[6]);
        if (sliderImage != null) {
            sliderImage.className = 'notdraggable';
        }
    }
}

// Returns variant mode (mvt,content personalization)
function GetCurrentObjectVariantMode(itemCode) {
    var itemInfo = itemCodesAssociativeArray[itemCode];
    if (itemInfo != null) {
        return itemInfo[11];
    }

    return '';
}

// Returns current variantId of the webpart/zone
function GetCurrentVariantId(itemCode) {
    var itemInfo = itemCodesAssociativeArray[itemCode];
    if (itemInfo != null) {
        // Get position info
        var sliderPosition = itemInfo[3];
        // Get VariantId
        return itemInfo[0][sliderPosition - 1];
    }

    return 0;
}

// Returns current variantId of the zoneId
function GetCurrentZoneVariantId(zoneId) {
    return GetCurrentVariantId("Variant_Zone_" + zoneId);
}

// Displays the selected variant. This method is called when there is a need of changing variant.
function SetVariant(itemCode, variantId) {
    var itemInfo = itemCodesAssociativeArray[itemCode];
    if (itemInfo != null) {
        // Get position info
        var hdnSliderPositionId = itemInfo[7];
        var hdnObj = document.getElementById(hdnSliderPositionId);
        if (hdnObj != null) {
            // Get position of the variantId
            var sliderPosition = ArrayIndexOf(itemInfo[0], variantId) + 1;

            // Reverse position index when in RTL
            if (variantSliderIsRTL) {
                sliderPosition = itemInfo[4] - (sliderPosition - 1);
            }

            // Set the last variant - if required
            if (variantId == -1) {
                // Set the last variant
                if (!variantSliderIsRTL) {
                    sliderPosition = itemInfo[0].length;
                }
                else {
                    // Last index in RTL
                    sliderPosition = 1;
                }
            }
            hdnObj.value = sliderPosition;
            $cmsj(hdnObj).change();
        }
    }
}

// Ensure that the context window will be within the browser's visible area
function AutoPostitionContextMenu(menuId) {
    var menuEl = $cmsj("#" + menuId);
    if (menuEl != null) {
        var position = menuEl.position();
        var rightPosition = position.left + menuEl.width();
        var bottomPosition = position.top + menuEl.height();
        var windowWidth = $cmsj(window).width() + $cmsj(window).scrollLeft();
        var windowHeight = $cmsj(window).height() + $cmsj(window).scrollTop();

        if ((rightPosition > windowWidth) || (position.left < 0)) {
            var newLeft = windowWidth - menuEl.width();
            if ((newLeft < 0) || (position.left < 0)) {
                newLeft = 0;
            }
            menuEl.css('left', newLeft);
        }
        if (bottomPosition > windowHeight) {
            var newTop = windowHeight - menuEl.height();
            if (newTop < 0) {
                newTop = 0;
            }
            menuEl.css('top', newTop);
        }
    }
}

// Show/hide context menu part relating to variants (Add variant, List all variants)
function OnSetContextMenuParameter(menuId, params) {
    var isWebPartMenu = (menuId == "webPartMenu") || (menuId == "cpVariantList");
    var isZoneMenu = (menuId == "webPartZoneMenu");


    if (isWebPartMenu || isZoneMenu) {

        var zoneId = params.zoneId;
        var instanceGuid = null;
        var displayMVTWebPartMenu = true;
        var displayMVTZoneMenu = true;
        var displayCPWebPartMenu = true;
        var displayCPZoneMenu = true;

        if (isWebPartMenu) {
            instanceGuid = params.instanceGuid;
        }

        // Check if variants menu is allowed
        if (isWebPartMenu && (ArrayIndexOf(zonesWithVariantsDenied, zoneId) >= 0)) {
            displayMVTWebPartMenu = false;
            displayCPWebPartMenu = false;
        } else if (!isWebPartMenu && (ArrayIndexOf(zonesWithVariantsDenied, '#' + zoneId) >= 0)) {
            displayMVTZoneMenu = false;
            displayCPZoneMenu = false;
        }
            // Show/Hide the "MVT variants"/"Personalization variants" context menu for a zone if there is already a MVT/Personalization web part variant (and vice versa for web part menu <=> zone variant)
        else {
            // Web part context menu
            if (isWebPartMenu) {
                // Find any variants related to the current web part or its parent zone
                for (var i in itemCodesAssociativeArray) {
                    var itemInfo = itemCodesAssociativeArray[i];
                    // Variant related to the current web part or its parent zone
                    if (itemInfo[9] == zoneId) {
                        // Parent zone variant => hide web part variants
                        if (itemInfo[10] == "") {
                            displayCPWebPartMenu = false;
                            displayMVTWebPartMenu = false;
                            break;
                        }
                            // Work only with the variant of the current web part
                        else if (itemInfo[10] == instanceGuid) {
                            // Is MVT web part variant => display only MVT variants
                            if (itemInfo[11] == "mvt") {
                                displayCPWebPartMenu = false;
                            }
                                // Is CP web part variant => display only CP variants
                            else {
                                displayMVTWebPartMenu = false;
                            }
                            break;
                        }
                    }
                }
            }
                // Zone context menu
            else {
                // Find any variants related to the current zone or its child web parts
                for (var i in itemCodesAssociativeArray) {
                    var itemInfo = itemCodesAssociativeArray[i];
                    // Variant related to the current zone or its child web parts
                    if (itemInfo[9] == zoneId) {
                        // Zone variant
                        if (itemInfo[10] == "") {
                            // Is MVT web part variant => display only MVT variants
                            if (itemInfo[11] == "mvt") {
                                displayCPZoneMenu = false;
                            }
                                // Is CP web part variant => display only CP variants
                            else {
                                displayMVTZoneMenu = false;
                            }
                        }
                            // Child web part variant => hide zone variants
                        else {
                            displayCPZoneMenu = false;
                            displayMVTZoneMenu = false;
                        }
                        break;
                    }
                }
            }
        }

        setSourceZoneVariantId(GetCurrentZoneVariantId(zoneId));
        if ((webPartMVTVariantContextMenuId != null) && (zoneMVTVariantContextMenuId != null)) {
            var wpMenu = document.getElementById(webPartMVTVariantContextMenuId);
            if (wpMenu != null) {
                wpMenu.style.display = (displayMVTWebPartMenu ? "block" : "none");
            }
            var zMenu = document.getElementById(zoneMVTVariantContextMenuId);
            if (zMenu != null) {
                zMenu.style.display = (displayMVTZoneMenu ? "block" : "none");
            }
        }
        if ((webPartCPVariantContextMenuId != null) && (zoneCPVariantContextMenuId != null)) {
            var wpMenu = document.getElementById(webPartCPVariantContextMenuId);
            if (wpMenu != null) {
                wpMenu.style.display = (displayCPWebPartMenu ? "block" : "none");
            }
            var zMenu = document.getElementById(zoneCPVariantContextMenuId);
            if (zMenu != null) {
                zMenu.style.display = (displayCPZoneMenu ? "block" : "none");
            }
        }
    }
}

// Updates the variant positions string
function UpdateVariantPosition(itemCode, variantId) {
    // Array containing slider positions in a format: sliderItemCode1:variantId1#sliderItemCode2:variantId2
    // variantId:
    //   -1: select the last variant (used when adding new variants)
    //    0: original web part/zone/widget
    //  1..: current variantId
    var sliderCodesArray = cpVariantSliderPositionElem.value.split("#");
    var found = false;
    // Try to find the corresponding slider position string
    for (var i = 0; i < sliderCodesArray.length; i++) {
        if (sliderCodesArray[i].length > 0) {
            var parts = sliderCodesArray[i].split(":");
            // Position string for the slider
            if (itemCode == parts[0]) {
                sliderCodesArray[i] = itemCode + ":" + variantId;
                found = true;
                break;
            }
        }
    }

    // Add the variant to the array
    if (!found) {
        // Clear the array if no slider configuration found
        if ((sliderCodesArray.length > 0) && (sliderCodesArray[0].length == 0)) {
            sliderCodesArray.length = 0;
        }
        // Add the slider configuration
        sliderCodesArray.push(itemCode + ":" + variantId);
    }

    // Save the code to the hidden field
    cpVariantSliderPositionElem.value = sliderCodesArray.join("#");

    // Ensure that the new variant will be chosen after the page refresh
    if (variantId == "-1") {
        variantSliderChanged = true;
    }
}

// Loads selected variant slider positions from the cookie
function SetSelectedVariants() {
    // Get the variantSliderPositionElem element from CMSPortalManager
    cpVariantSliderPositionElem = GetCPVariantSliderPositionElem();
    var sliderCodesArray = cpVariantSliderPositionElem.value.split("#");
    var indexToRemove = -1;

    // Loop through the all Content personalization variant slider codes and set the saved slider positions
    for (var i = 0; i < sliderCodesArray.length; i++) {
        if (sliderCodesArray[i].length > 0) {
            var parts = sliderCodesArray[i].split(":");
            if ((parts.length == 2) && (parts[0] in itemCodesAssociativeArray)) {
                // Set the slider position
                SetVariant(parts[0], parseInt(parts[1]));

                // If the array contains a configuration for MVT webpart/widget/zone, save its index. It will be deleted later
                if (itemCodesAssociativeArray[parts[0]][11] == 'mvt') {
                    indexToRemove = i;
                }
            }
        }
    }

    // Delete the MVT variant configuration - this happened when adding or deleting a MVT variant
    if (indexToRemove > -1) {
        sliderCodesArray.splice(indexToRemove, 1);
        if (window.UpdateCombinationPanel) {
            UpdateCombinationPanel();
        }

        // Save the code to the hidden field
        cpVariantSliderPositionElem.value = sliderCodesArray.join("#");
    }
}

// Gets the array containing variant information
function GetVariantInfoArray(obj) {
    var arr = new Array();
    arr.push(obj.zoneId);
    arr.push(obj.webPartId);
    arr.push(obj.nodeAliasPath);
    arr.push(obj.instanceGuid);

    return arr;
}