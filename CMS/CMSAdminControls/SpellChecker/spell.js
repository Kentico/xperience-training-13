/****************************************************
* Spell Checker Client JavaScript Code
****************************************************/

var showCompleteAlert = true;


var tagGroup = ["INPUT", "TEXTAREA", "DIV", "SPAN"];
// global elements to check
var checkElements = [];
var checkEditors = [];

var oElement = null;
var oEditor = null;
var spellURL;

function allowSpellCheck(elementId) {
    if (window.spellCheckFields == null) {
        return true;
    }

    for (var i = 0; i < spellCheckFields.length; i++) {
        if (spellCheckFields[i] == elementId) {
            return true;
        }
    }
    return false;
}

function getText(index) {
    var sText = "";

    // Editors 
    if (index >= checkElements.length) {
        oEditor = checkEditors[index - checkElements.length];
        if (allowSpellCheck(oEditor.Name)) {
            sText = oEditor.getData();
        }
    }
    else // Regular fields
    {
        if (allowSpellCheck(checkElements[index]) && (checkElements[index] != "")) {
            oElement = document.getElementById(checkElements[index]);

            switch (oElement.tagName) {
                case "INPUT":
                case "TEXTAREA":
                    // Ensure CKEditor correct text get
                    if (typeof (CKEDITOR) !== 'undefined') {
                        var oEditor = CKEDITOR.instances[oElement.id];
                        if (oEditor) {
                            sText = oEditor.getData();
                            break;
                        }
                    }
                    sText = oElement.value;
                    break;
                case "DIV":
                case "SPAN":
                case "BODY":
                    sText = oElement.innerHTML;
                    break;
                case "IFRAME":
                    if (document.all) // IE
                    {
                        var oFrame = eval(oElement.id);
                        sText = oFrame.document.body.innerHTML;
                    }
                    else {
                        sText = parent.frames[0].document.body.innerHTML;
                    }
                    break;
            }
        }
    }

    return sText;
}

function setText(index, text) {
    // Editors
    if (index >= checkElements.length) {
        oEditor = checkEditors[index - checkElements.length];
        sText = oEditor.setData(text);
    }
    else // Regular fields
    {
        oElement = document.getElementById(checkElements[index]);

        switch (oElement.tagName) {
            case "INPUT":
                oElement.value = text;
            case "TEXTAREA":
                // Ensure CKEditor correct text set
                if (typeof (CKEDITOR) !== 'undefined') {
                    var oEditor = CKEDITOR.instances[oElement.id];
                    if (oEditor) {
                        oEditor.setData(text);
                        return;
                    }
                }
                oElement.value = text;
                break;
            case "DIV":
            case "SPAN":
                oElement.innerHTML = text;
                break;
            case "IFRAME":
                if (document.all) // IE
                {
                    var oFrame = eval(oElement.id);
                    oFrame.document.body.innerHTML = text;
                }
                else {
                    parent.frames[0].document.body.innerHTML = text;
                }
                break;
        }
    }
}

function checkSpelling(spellURLparam) {
    spellURL = spellURLparam;
    checkElements = [];
    checkEditors = [];

    // loop through all tag groups
    for (var i = 0; i < tagGroup.length; i++) {
        var sTagName = tagGroup[i];
        var oElements = document.getElementsByTagName(sTagName);
        // loop through all elements
        for (var x = 0; x < oElements.length; x++) {
            if ((sTagName == "INPUT" && oElements[x].type == "text") || ((sTagName == "TEXTAREA") && (oElements[x].className != "CKEditorArea")))
                checkElements[checkElements.length] = oElements[x].id;
            else if ((sTagName == "DIV" || sTagName == "SPAN") && oElements[x].isContentEditable)
                checkElements[checkElements.length] = oElements[x].id;
        }
    }

    // loop through all editors
    try {
        if (typeof (CKEDITOR) !== 'undefined') {
            for (var name in CKEDITOR.instances) {
                if (isInElements(name)) {
                    break;
                }
                checkEditors[checkEditors.length] = CKEDITOR.instances[name];
            }
        }
    }
    catch (ex) {
    }

    openSpellChecker();
}

function isInElements(editorName) {
    if (checkElements.length > 0) {
        for (var i = 0; i < checkElements.length; i++) {
            if (checkElements[i] == editorName) {
                return true;
            }
        }
    }
    return false;
}

function checkSpellingById(id) {
    checkElements = new Array();
    checkElements[checkElements.length] = id;
    openSpellChecker();
}

function checkElementSpelling(element) {
    checkElements = new Array();
    checkElements[checkElements.length] = element.id;
    openSpellChecker();
}

function openSpellChecker() {
    var dHeight = 410;
    if ((document.all) && (navigator.appName != 'Opera')) {
        if (parseInt(navigator.appVersion.substr(22, 1)) < 7) { dHeight += 58; };
    }

    if (window.modalDialog)
        var result = modalDialog(spellURL, "spellcheck", 400, 480);
    else
        var newWindow = window.open(spellURL, "newWindow", "height=" + 480 + ",width=400,scrollbars=no,resizable=no,toolbars=no,status=no,menubar=no,location=no");
}


/****************************************************
* Spell Checker Suggestion Window JavaScript Code
****************************************************/
var iElementIndex = -1;
var parentWindow;
var iCKIndex = -1;
var wordIndexElem;
var currentTextElem;
var elementIndexElem;
var spellModeElem;
var statusTextElem;
var replacementWordElem;

function initialize(wordIndexID, currentTextID, elementIndexID, spellModeID, statusTextID, replacementWordID, checkText, failText) {
    parentWindow = wopener;

    wordIndexElem = document.getElementById(wordIndexID);
    currentTextElem = document.getElementById(currentTextID);
    elementIndexElem = document.getElementById(elementIndexID);
    spellModeElem = document.getElementById(spellModeID);
    statusTextElem = document.getElementById(statusTextID);
    replacementWordElem = document.getElementById(replacementWordID);

    iElementIndex = parseInt(elementIndexElem.value);

    switch (spellModeElem.value) {
        case "start":
            // do nothing client side
            break;
        case "suggest":
            // update text from parent document
            updateText();
            // wait for input
            break;
        case "end":
            // update text from parent document
            updateText();
            // fall through to default
        default:
            // get text block from parent document
            if (loadText(checkText)) {
                document.forms[0].submit();
            } else {
                statusTextElem.innerHTML = failText;
            }
            break;
    }
}

function loadText(checkStatus) {
    try {
        if (!parentWindow.document || !parentWindow.checkElements)
            return false;
    }
    catch (ex) {
        return false;
    }

    // check if there is any text to spell check
    for (++iElementIndex; iElementIndex < parentWindow.checkElements.length + parentWindow.checkEditors.length; iElementIndex++) {
        var newText = parentWindow.getText(iElementIndex);
        if ((newText != null) && (newText.length > 0)) {
            updateSettings(newText, 0, iElementIndex, "start");
            statusTextElem.innerHTML = checkStatus;
            return true;
        }
    }

    return false;
}

function updateSettings(currentText, wordIndex, elementIndex, mode) {
    currentTextElem.value = currentText;
    wordIndexElem.value = wordIndex;
    elementIndexElem.value = elementIndex;
    spellModeElem.value = mode;
}

function updateText() {
    try {
        if (!parentWindow.document || !parentWindow.setText)
            return false;
    }
    catch (ex) {
        return false;
    }

    var newText = currentTextElem.value;
    parentWindow.setText(iElementIndex, newText);
}

function changeWord(element) {
    var k = element.selectedIndex;
    replacementWordElem.value = element.options[k].value;
}