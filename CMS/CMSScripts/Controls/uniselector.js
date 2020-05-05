function US_InitDropDown(control) {
    if (control != null) {
        control.originalValue = control.value;
    }
}

function US_SetVal(id, val) {
    if ((id != null) && (id != '')) {
        var el = document.getElementById(id);
        if (el != null) {
            if (el.TextBoxWrapper != null) {
                el.TextBoxWrapper.set_Value(val);
            }
            else {
                el.value = val;
            }

            US_Changed();
        }
    }
}

function US_GetVal(id) {
    if ((id != null) && (id != '')) {
        var el = document.getElementById(id);
        if (el != null) {
            if (el.TextBoxWrapper != null) {
                return el.TextBoxWrapper.get_Value();
            }
            else {
                return el.value;
            }
        }
    }

    return '';
}

function US_Changed() {
    if (window.Changed != null) {
        window.Changed();
    }
}

function US_ItemChanged(control, clientId) {
    if (control != null) {
        var editButton = document.getElementById(clientId + '_btnDropEdit_btn');
        if (editButton != null) {
            setTimeout('BTN_Enable(\"' + clientId + '_btnDropEdit_btn\");', 1);
        }

        var disableButton = false;
        var result = true;
        var restoreSelection = false;

        if ((control.value == '-1') || (control.value == '0')) {
            disableButton = true;
        }
        else if (control.value == '-2') {
            setTimeout('US_SelectionDialog_' + clientId + '();', 1);
            disableButton = true;
            restoreSelection = true;
            result = false;
        }
        else if (control.value == '-3') {
            setTimeout('US_NewItem_' + clientId + '(\'' + control.value + '\');', 1);
            disableButton = false;
            restoreSelection = true;
            result = false;
        }
        else if (control.value == 'default') {
            result = false;
        }

        if (restoreSelection) {
            if (control.originalValue != null) {
                control.value = control.originalValue;
                var setHashFn = window['SetHash_' + clientId];
                if (setHashFn) {
                    setHashFn(control);
                }
                if ((control.value == '-2') || (control.value == '-1') || (control.value == '0')) {
                    disableButton = true;
                }
            }
        }
        else {
            control.originalValue = control.value;
        }
        if (disableButton && (editButton != null)) {
            setTimeout('BTN_Disable(\"' + clientId + '_btnDropEdit_btn\");', 1);
        }
        return result;
    }
    return true;
}

function US_SetItems(items, names, hiddenFieldID, txtClientID, hidValue, hidHash, hash) {
    US_SetVal(txtClientID, decodeURIComponent(names));
    US_SetVal(hiddenFieldID, decodeURIComponent(items));
    US_SetVal(hidValue, decodeURIComponent(items));
    US_SetVal(hidHash, hash);
    
    return false;
}

function escapeRegExp(string) {
    // Escape the following special characters for regular expression: \ ^ $ * + ? . ( ) | { } [ ]
    return string.replace(/[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g, "\\$&");
}

function US_ProcessItem(clientId, valuesSeparator, chkbox, changeChecked) {
    var itemsElem = document.getElementById(clientId + '_hiddenSelected');
    var items = itemsElem.value;
    var item = chkbox.id.substr(36);
    if (changeChecked) {
        chkbox.checked = !chkbox.checked;
    }
    if (chkbox.checked) {
        if (items == '') {
            items = valuesSeparator + item + valuesSeparator;
        }
        else if (items.toLowerCase().indexOf(valuesSeparator + item.toLowerCase() + valuesSeparator) < 0) {
            items += item + valuesSeparator;
        }
    }
    else {
        var pattern = escapeRegExp(valuesSeparator + item + valuesSeparator);
        var re = new RegExp(pattern, 'i');
        items = items.replace(re, valuesSeparator);
    }
    itemsElem.value = items;
}

function US_SelectAllItems(clientId, valuesSeparator, checkbox, checkboxClass) {
    var checkboxes = document.getElementsByTagName('input');
    for (var j = 0; j < checkboxes.length; j++) {
        var chkbox = checkboxes[j];
        if (chkbox.className == checkboxClass) {
            if (checkbox.checked) { chkbox.checked = true; }
            else { chkbox.checked = false; }

            US_ProcessItem(clientId, valuesSeparator, chkbox);
        }
    }
}

function US_TrimSeparators(value, valueSeparator) {
    if ((typeof value) == 'string') {
        if (value.charAt(0) == valueSeparator) {
            value = value.substring(1);
        }
        if (value.charAt(value.length - 1) == valueSeparator) {
            value = value.substring(0, value.length - 1);
        }
    }

    return value;
}