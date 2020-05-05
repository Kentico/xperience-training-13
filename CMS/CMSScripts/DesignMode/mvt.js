//////////////////////////////
// CMS variants functions
//////////////////////////////

// Show variant edit dialog
function AddMVTVariant(zoneId, webPartName, aliasPath, instanceGuid, templateId, variantType, itemCode) {
    var url = GetMVTAddVariantDialog() + "&zoneid=" + zoneId + "&webpartid=" + webPartName + "&instanceguid=" + instanceGuid + "&aliaspath=" + aliasPath + "&templateid=" + templateId + "&varianttype=" + variantType;
    modalDialog(url, 'editmvtvariant', 700, 400);
}

// Show variant edit dialog
function ListMVTVariants(zoneId, webPartName, aliasPath, instanceGuid, templateId, variantType, itemCode) {
    var url = GetMVTListVariantsDialog() + "&zoneid=" + zoneId + "&webpartid=" + webPartName + "&instanceguid=" + instanceGuid + "&aliaspath=" + aliasPath + "&templateid=" + templateId + "&varianttype=" + variantType;
    modalDialog(url, 'editmvtvariant', 630, 540);
}

// Updates the combination panel according to the currently displayed variants
function UpdateCombinationPanel() {
    // Code for the current (selected) variants - this code will be used to detect the selected combination (each combination contains a unique code of its variant IDs)
    // (format: 155_158_180)
    var variantIDsCode = '';
    // Temporary array which is used for sorting variant IDs (to help generating the 'variantIDsCode')
    var variantIDsArray = new Array();

    for (var i in itemCodesAssociativeArray) {
        // if the variant is not the original then add its ID to the array to be sorted
        var itemInfo = itemCodesAssociativeArray[i];
        // Process the MVT variants only
        if (itemInfo[11] == "mvt") {
            var sliderPosition = itemInfo[3];
            if (sliderPosition > 1) {
                variantIDsArray.push(itemInfo[0][sliderPosition - 1]);
            }
        }
    }

    // Add the compulsory variants (i.e. if in Edit mode -> add all web part/zone variants for the chosed combination to ensure
    // that the CombinationPanel will display the correct combination when changing variant sliders)
    for (var i = 0; i < compulsoryCombinationVariants.length; i++) {
        variantIDsArray.push(compulsoryCombinationVariants[i]);
    }

    // Sort the variant IDs array
    variantIDsArray.sort();

    // Generate the unique variant IDs code (format: 155_158_180)
    for (var i = 0; i < variantIDsArray.length; i++) {
        if (variantIDsCode.length > 0) {
            variantIDsCode += '_';
        }

        variantIDsCode += variantIDsArray[i];
    }

    // Get the the actual combination
    var currentCombination = null;
    for (var i = 0; i < combinationsArray.length; i++) {
        if (variantIDsCode == combinationsArray[i][3]) {
            currentCombination = combinationsArray[i];
            break;
        }
    }

    // Change the combination panel values
    if (currentCombination != null) {
        mvtCPselector.value = currentCombination[0];
    }
    // Change the rest of the panel only when displayed
    if ((currentCombination != null)
        && (mvtCPenabled != null)
        && (mvtCPcustomName != null)
        && (mvtCPcurrentCombinationName != null)) {
        mvtCPenabled.checked = currentCombination[1];
        mvtCPcustomName.value = currentCombination[2];
        mvtCPcurrentCombinationName.value = currentCombination[0];
    }
}