// Insert desired HTML at the current cursor position of the CK editor or CodeMirror editor
function InsertHTML(htmlString) {
    if (window['CKEDITOR'] != undefined) {
        // Get the editor instance that we want to interact with.
        var oEditor = CKEDITOR.instances[ckEditorID];

        // Check the active editing mode.
        if (oEditor.mode == 'wysiwyg') {
            // Insert the desired HTML.
            oEditor.insertHtml(htmlString);
        }
        else {
            alert('You must be on WYSIWYG mode!');
        }
    } else if (window['CM_instances'] != undefined && CM_instances != null) {
        var editor = CM_instances[0];
        if (editor != null) {
            editor.replaceSelection(htmlString);
            editor.focus();
        }
    }
}

// Set content of the CK editor or CodeMirror editor - replace the actual one
function SetContent(newContent) {
    if (window['CKEDITOR'] != undefined) {
        var oEditor = CKEDITOR.instances[ckEditorID];
        oEditor.setData(newContent);
    } else if (window['CM_instances'] != undefined && CM_instances != null) {
        var editor = CM_instances[0];
        if (editor != null) {
            editor.setValue(newContent);
        }
    }
}

// Get content of the CK editor or CodeMirror editor
function GetContent() {
    if (window['CKEDITOR'] != undefined) {
        var oEditor = CKEDITOR.instances[ckEditorID];
        return oEditor.getData();
    } else if (window['CM_instances'] != undefined && CM_instances != null) {
        var editor = CM_instances[0];
        if (editor != null) {
            return editor.getValue();
        }
    }
}

// Returns HTML code with standard table layout
function GenerateHtmlLayout() {
    var htmlLayout = "";

    // indicates whether any row definition was added to the table
    var rowAdded = false;

    // list of attributes
    var list = drpAvailableFields;

    // attributes count
    var optionsCount = list.options.length;

    for (var i = 0; i < optionsCount; i++) {
        htmlLayout += "<tr class=\"form-table-group\"><td class=\"form-table-label-cell\">$$label:" + list.options[i].value + "$$</td><td class=\"form-table-value-cell\">$$input:" + list.options[i].value + "$$</td><td class=\"form-table-validation-cell\">$$validation:" + list.options[i].value + "$$</td></tr>";
        rowAdded = true;
    }

    if (rowAdded) {
        htmlLayout = "<table class=\"form-table\"><tbody>" + htmlLayout + "</tbody></table>";
    }

    return htmlLayout;
}

function GenerateAscxLayout() {
    var ascxLayout = "";

    // list of attributes
    var list = drpAvailableFields;

    // attributes count
    var optionsCount = list.options.length;

    for (var i = 0; i < optionsCount; i++) {
        ascxLayout += '<cms:FormField runat=\"server\" ID="f' + list.options[i].value + '" Field="' + list.options[i].value + '" />\n';
    }

    ascxLayout += "<cms:FormSubmit runat=\"server\" ID=\"fSubmit\" />";

    return ascxLayout;
}

// Determines whether specified html string is already in CK editing window or not
function IsInContent(content, htmlString) {
    return (content.toLowerCase().indexOf(htmlString.toLowerCase()) != -1);
}

// Determines whether specified html string is already in CK editing window or not
function IsInContentMoreThanOnce(content, htmlString) {
    return (content.toLowerCase().indexOf(htmlString.toLowerCase()) != content.toLowerCase().lastIndexOf(htmlString.toLowerCase()));
}

// Insert desired HTML at the current cursor position of the CK editor if it is not already inserted 
function InsertAtCursorPosition(htmlString) {
    var content = GetContent();

    // doesnt already exist -> insert
    if (!IsInContent(content, htmlString)) {
        InsertHTML(htmlString);
    }
        // already exists -> alert
    else {
        alert(msgAltertExist + " '" + htmlString + "'");
    }
}

function ButtonInsert() {
    switch (drpLayoutType.value) {
        case "Html":
            InsertAsHtml();
            break;

        case "Ascx":
            InsertAsAscx();
            break;

        default:
            alert("There is no layout type selected");
            break;
    }
}

// On drpFieldType change event. Hide available fields and label when submit button selected.
function ShowHideAvailableFields() {
    if (drpFieldType.value == 'submitbutton') {
        drpAvailableFields.disabled = 1;
        lblForField.disabled = 1;
    }
    else {
        drpAvailableFields.disabled = 0;
        lblForField.disabled = 0;
    }
}

function InsertAsHtml() {
    switch (drpFieldType.value) {
        case "submitbutton":
            InsertAtCursorPosition('$$submitbutton$$');
            break;

        default:
            InsertAtCursorPosition('$$' + drpFieldType.value + ':' + drpAvailableFields.value + '$$');
            break;
    }
}

function InsertAsAscx() {
    switch (drpFieldType.value) {
        case "field":
            InsertAtCursorPosition('<cms:FormField runat=\"server\" ID="f' + drpAvailableFields.value + '" Field="' + drpAvailableFields.value + '" />\n');
            break;

        case "submitbutton":
            InsertAtCursorPosition('<cms:FormSubmit runat="server" ID="fSubmit" />');
            break;

        case "label":
            InsertAtCursorPosition('<cms:FormLabel runat=\"server\" ID="l' + drpAvailableFields.value + '" Field="' + drpAvailableFields.value + '" />');
            break;

        case "errorlabel":
            InsertAtCursorPosition('<cms:FormErrorLabel runat=\"server\" ID="e' + drpAvailableFields.value + '" Field="' + drpAvailableFields.value + '" />');
            break;

        case "input":
            InsertAtCursorPosition('<cms:FormControl runat=\"server\" ID="i' + drpAvailableFields.value + '" Field="' + drpAvailableFields.value + '" FormControlName="' + formControlName[drpAvailableFields.value] + '" CssClass="' + formControlCssClass[drpAvailableFields.value] + '" />');
            break;
    }
}

// Checks if field items are only once in CK editor content
function CheckContent() {
    var content = GetContent();

    // list of attributes
    var list = drpAvailableFields;

    // attributes count
    var optionsCount = list.options.length;

    // array of field Items
    var fieldItems = new Array(3);

    // error mesaage to display
    var errorMessage = "";

    fieldItems[0] = "label:";
    fieldItems[1] = "input:";
    fieldItems[2] = "validation:";


    // for each field
    for (var i = 0; i < optionsCount; i++) {
        // for each field item
        for (var j = 0; j < 3; j++) {
            // string to check
            htmlString = "$$" + fieldItems[j] + list.options[i].value + "$$";

            if (IsInContentMoreThanOnce(content, htmlString)) {
                if (errorMessage == "") {
                    errorMessage = msgAlertExistFinal + "\n";
                }
                errorMessage += "'" + htmlString + "', ";
            }
        }
    }

    htmlString = "$$submitbutton$$";
    if (IsInContentMoreThanOnce(content, htmlString)) {
        if (errorMessage == "") {
            errorMessage = msgAlertExistFinal + "\n";
        }
        errorMessage += "'" + htmlString + "', ";
    }

    if (errorMessage != "") {
        // remove ending comma ", " from error string                
        errorMessage = errorMessage.substring(0, errorMessage.length - 2);

        // display error message
        alert(errorMessage);

        // avoid sending form data          
        return false;
    }
    else {
        // send form data
        return true;
    }
}

function ConfirmDelete() {
    return confirm(msgConfirmDelete);
}