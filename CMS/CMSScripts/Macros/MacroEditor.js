// KeyCode constants
var TAB = 9;
var ENTER = 13;
var ESC = 27;
var PGUP = 33;
var PGDOWN = 34;
var HOME = 35;
var END = 36;
var KEYUP = 38;
var KEYDOWN = 40;
var KEYLEFT = 37;
var KEYRIGHT = 39;
var CTRL = 17;
var BACKSPACE = 8;
var DELETE = 46;
var DOT_KEYCODE = 190;

// CharCode constants
var SPACE = 32;
var PERCENT = 37;
var COMMA = 44;
var DOT = 46;
var DQUOTE = 58;
var SEMICOLON = 59;
var LEFTBRACKET = 40;
var RIGHTBRACKET = 41;
var LEFTINDEXER = 91;
var RIGHTINDEXER = 93;
var RIGHTCURLY = 125;

// Regex
var ALPHANUMERIC_REGEX = '[a-zA-Z0-9_]';
var MAX_ZINDEX = 2147483647;

function AutoCompleteExtender(codeElem, hintElem, contextElem, quickContextElem, mixedMode, editorTopOffset, editorLeftOffset, ascxMode) {

    // Local variables
    var unit = "px";
    var offset = 6;

    // The 'me' variable allow you to access the AutoSuggest object
    // from the elem's event handlers defined below.
    var me = this;

    // Items displayed in "limited output" mode
    this.displayedItems = null;

    // List of currently used elements
    this.currentItems = [];

    // If true, auto completion is always shown above the cursor
    this.forceAbove = false;

    // Show hint below the cursor
    this.isHintDown = true;

    // Save editor offset
    this.topOffset = editorTopOffset;
    this.leftOffset = editorLeftOffset;

    // A reference to the element we're binding the autocompletion to
    this.elem = codeElem;

    if (this.elem.lineContent == null) {
        this.elem.lineContent = this.elem.getLine;
    }
    if (this.elem.cursorLine == null) {
        this.elem.cursorLine = function () { return this.getCursor().line; };
    }
    if (this.elem.frame == null) {
        this.elem.frame = this.elem.getInputField();
    }

    // Indicates whether the editor is in pure macro editing mode (whole text is considered as macro) 
    // or mixed mode, where auto completion is fired only inside {%%} environment.
    this.isMixedMode = mixedMode;

    // A div to use to create the dropdown.
    this.hintsDiv = hintElem;
    this.hintsDiv.style.zIndex = MAX_ZINDEX;

    // Register scroll event handler that recalculates divs positions
    $cmsj(this.hintsDiv).parents().scroll(function (e) {
        me.positionDivs();
    });

    $cmsj(window).scroll(function (e) {
        me.positionDivs();
    });

    // A div to use to display method context help
    this.contextDiv = contextElem;
    this.contextDiv.style.zIndex = MAX_ZINDEX;

    // A div to use to display quick method context help (when browsing through hints)
    this.quickContextDiv = quickContextElem;
    this.quickContextDiv.style.zIndex = MAX_ZINDEX;

    // Array storing relevant hints (filled with callback data)
    this.hints = new Array();

    // Field storing the current context (filled with callback data)
    this.context = '';

    // Field used to pass arguments to callback functions
    this.callbackArgument = null;

    // Flag for CTRL+SPACE if there is only one hint, it will be used automatically
    this.autoComplete = false;

    // If true, next normal key (alphanumeric) will cause the hints to be displayed
    this.showHintNextTime = true;

    // If true, hints are shown after dot is pressed (applies only to ASCX mode)
    this.showHintAfterDotASCX = false;

    // A pointer to the index of the highlighted eligible item. -1 means nothing highlighted.
    this.highlighted = -1;

    // Current key code stored for key event handlers
    this.currentKeyCode = null;

    // Keeps the information whether quick context is displayed on the left
    this.quickContextLeft = false;

    // This flag is to ensure that lost focus because of clicking the option from hint with mouse does not cause the hint to hide too early
    this.stillHasFocus = false;

    // Register the events
    if (this.elem.win != null) {
        this.docObj = $cmsj(this.elem.win.document);
    }
    else {
        this.docObj = $cmsj(this.elem.getInputField());
    }

    this.docObj.keydown(function (e) {
        me.handleKeyDown(e);
    });
    this.docObj.keyup(function (e) {
        me.handleKeyUp(e);
    });
    this.docObj.keypress(function (e) {
        me.handleKeyPress(e);
    });

    this.elem.setOption('onKeyEvent', function (instance, event) {
        if (event.type == 'keydown') {
            // Handle key down from code mirror control
            me.handleCodeMirrorKeyDown(event);
        }
    });

    this.elem.setOption('onMouseDown', function (instance, event) {
        me.handleMouseDown(event);
    });

    // Events that handle hiding of the autocompletion when the focus is outside the editor
    this.frameObj = $cmsj(this.elem.frame);
    this.frameObj.blur(function (e) {
        if (!me.stillHasFocus) {
            me.hideAutoCompletion();
            me.editorFocusLost();
        }
    });

    this.docObj.blur(function (e) {
        if (!me.stillHasFocus) {
            me.hideAutoCompletion();
            me.editorFocusLost();
        }
    });

    this.hintsObj = $cmsj(this.hintsDiv);
    this.hintsObj.mouseover(function (e) {
        me.stillHasFocus = true;
    });
    this.hintsObj.mouseout(function (e) {
        me.stillHasFocus = false;
    });


    /*
    * Keyboard handlers.
    */

    // Key down event called from code mirror
    this.handleCodeMirrorKeyDown = function (ev) {
        var key = me.getKeyCode(ev);

        switch (key) {
            case BACKSPACE:
                if (me.isHintsDivDisplayed()) {
                    // We need to find out if we deleted the dot, if so, IntelliSense should be hidden
                    var pos = this.currentLinePos();
                    if (pos > 0) {
                        var charToDelete = this.currentLineText()[pos - 1];
                        if (charToDelete == '.') {
                            me.hideHints(true);
                        }
                    }
                }
                break;
        }
    }


    this.handleKeyDown = function (ev) {
        var key = me.getKeyCode(ev);

        // Store current key code
        me.currentKeyCode = key;
        
        switch (key) {
            case END:
            case HOME:
            case ESC:
                {
                    me.hideHints(false);
                    me.hideContext();
                    me.hideQuickContext();
                }
                break;

            case KEYLEFT:
            case KEYRIGHT:
                if (this.autoCompleteEnabled()) {
                    me.hideHints(true);
                    me.showContext();
                }
                break;

            case KEYUP:
                if (me.changeHighlightPosition(true)) {
                    me.changeHighlight(true);
                    return me.cancelEvent(ev);
                }
                break;

            case KEYDOWN:
                if (me.changeHighlightPosition(false)) {
                    me.changeHighlight(true);
                    return me.cancelEvent(ev);
                }
                break;

            case PGUP:
                if (me.changeHighlightPosition(true, true)) {
                    me.changeHighlight(true);
                    return me.cancelEvent(ev);
                }
                break;

            case PGDOWN:
                if (me.changeHighlightPosition(false, true)) {
                    me.changeHighlight(true);
                    return me.cancelEvent(ev);
                }
                break;

            case SPACE:
                // Force show hint
                if (this.autoCompleteEnabled()) {
                    if (ev.ctrlKey) {
                        me.autoComplete = true;
                        me.showHints(null);
                        return me.cancelEvent(ev);
                    }
                }
                break;
        }

        if (key == ESC) {
            return me.cancelEvent(ev);
        }
    };

    /*
     *   Changes highlighted index with dependence on required direction and step size
     */
    this.changeHighlightPosition = function (up, bigStep) {

        // Check whether context help is displayed
        if (me.isHintsDivDisplayed()) {

            // Set step size
            var stepSize = 1;
            if (bigStep) {
                stepSize = 9;
            }

            // Gets current index defined by limited output
            var limitedIndex = -1;
            if (me.displayedItems != null) {
                limitedIndex = me.displayedItems.indexOf(me.highlighted);
            }

            // Move up
            if (up) {

                // The index cannot be negative
                if (me.highlighted > 0) {

                    // decrease the highlighted value for non-limited output 
                    if (limitedIndex == -1) {
                        me.highlighted = Math.max(0, me.highlighted - stepSize);
                    }
                        // Get previous highlighted item from limited output
                    else {
                        var nextIndex = Math.max(0, limitedIndex - stepSize);
                        me.highlighted = me.displayedItems[nextIndex];
                    }
                }
            }
                // Move down
            else {

                var maxListLength = (me.hints.length - 1);

                // The index cannot be higher then max index of the current list
                if (me.highlighted < maxListLength) {

                    // increase the highlighted value for non-limited output 
                    if (limitedIndex == -1) {
                        me.highlighted = Math.min(maxListLength, me.highlighted + stepSize);
                    }
                        // Get the next highlighted item from limited output
                    else {
                        var nextIndex = Math.min(me.displayedItems.length - 1, limitedIndex + stepSize);
                        me.highlighted = me.displayedItems[nextIndex];
                    }

                }

            }
            return true;
        }
        return false;
    };

    this.handleKeyUp = function (ev) {
        var keyCode = me.getKeyCode(ev);
        if (this.autoCompleteEnabled()) {
            // Use hint on ENTER or TAB when hints are displayed (for Chrome & IE - they do not handle it on KeyPress)
            this.handleEnterTab(keyCode, ev);

            switch (keyCode) {
                case BACKSPACE:
                case DELETE:
                    me.showContext(null);
                    me.findHint(null, true);
                    break;
            }
        }
    };

    this.handleKeyPress = function (ev) {
        if (this.autoCompleteEnabled()) {
            var key = me.getCharCode(ev);
            var keyCode = me.getKeyCode(ev);
            var wasUseHint = false;
            var character = String.fromCharCode(key);

            // Use hint on ENTER or TAB when hints are displayed
            this.handleEnterTab(keyCode, ev);
            switch (keyCode) {
                case ENTER:
                case TAB:
                    wasUseHint = true;
            }

            // For some keys use hint should be triggered
            switch (key) {

                case SPACE:
                    if (!ev.ctrlKey) {
                        if (me.isHintsDivDisplayed()) {
                            me.useHint();
                            wasUseHint = true;
                        }
                    }
                    break;

                case SEMICOLON:
                case DOT:
                case COMMA:
                case LEFTBRACKET:
                case RIGHTBRACKET:
                case LEFTINDEXER:
                    me.useHint();
                    wasUseHint = true;
                    break;
            }

            switch (key) {

                case 0:
                    // Special control keys - do nothing
                    return;

                case RIGHTBRACKET:
                case RIGHTINDEXER:
                case COMMA:
                case LEFTINDEXER:
                case LEFTBRACKET:
                    me.hideHints(true);
                    me.showContext(character);
                    break;

                case SEMICOLON:
                    me.hideHints(true);
                    break;

                case PERCENT:
                case RIGHTCURLY:
                case DQUOTE:
                    me.hideHints(false);
                    break;

                case DOT:
                    if (me.showHintsAfterDot()) {
                        me.showHints(character);
                    }
                    break;

                default:
                    // If we have alphanumeric char we might need to show the hints
                    if (!ascxMode && me.showHintNextTime && !me.isHintsDivDisplayed() && character.match(ALPHANUMERIC_REGEX)) {
                        me.showHints(character);
                        me.showHintNextTime = false;
                    }
                    me.findHint(character, false);
                    break;
            }

            if ((key != DOT) && !wasUseHint) {
                me.showHintAfterDotASCX = false;
            }

        } else {
            this.hideHints();
            this.hideQuickContext();
            this.hideContext();
        }
    };

    this.handleEnterTab = function (keyCode, ev) {
        switch (keyCode) {
            case ENTER:
                if (this.isHintsDivDisplayed()) {
                    this.useHint();
                    return this.cancelEvent(ev);
                }
                break;

            case TAB:
                if (this.isHintsDivDisplayed()) {
                    this.useHint(true);
                    return this.cancelEvent(ev);
                }
                break;
        }
    };

    this.handleMouseDown = function (ev) {
        this.hideHints();
        this.hideQuickContext();
        this.hideContext();
    };

    // Prevent hiding intellisense when clicking on the Quick Context div
    this.preventHiding = function (elem) {
        $cmsj(elem).bind("mousedown click focus", function (e) {
            e.preventDefault();
        });
    };
    this.preventHiding(me.contextDiv);
    this.preventHiding(me.quickContextDiv);

    // Remove field title if hints or Context tooltips are shown
    var tempTitle = $cmsj(me.hintsDiv).closest('.form-group').map(function () {
        return this.title;
    }).get();

    tooltipsWithMouseover = "#" + me.contextDiv.id + ",#" + me.quickContextDiv.id + ",#" + me.hintsDiv.id;
    $cmsj(tooltipsWithMouseover).bind('mouseover', function (e) {
        $cmsj(me.hintsDiv).closest('.form-group').attr('title', '');
    });
    $cmsj(tooltipsWithMouseover).bind('mouseout', function (e) {
        $cmsj(me.hintsDiv).closest('.form-group').attr('title', tempTitle);
    });


    // Hides all the divs
    this.hideAutoCompletion = function () {
        this.hideHints(true);
        this.hideContext();
        this.hideQuickContext();
    };

    // Determines whether to show hints when a dot is pressed
    this.showHintsAfterDot = function () {
        return !ascxMode || this.showHintAfterDotASCX;
    };

    /*
    * Method quick context help methods.
    */

    // Hides the method context.
    this.hideQuickContext = function () {
        this.hideQuickContextDiv();
    };

    // Shows the hints div.
    this.showQuickContextDiv = function () {
        this.quickContextDiv.style.display = 'block';
    };

    // Hides the hints div.
    this.hideQuickContextDiv = function () {
        this.quickContextDiv.style.display = 'none';
    };

    // Creates context div.
    this.createQuickContextDiv = function (help) {

        this.quickContextDiv.innerHTML = help;
        this.quickContextDiv.className = "auto-complete-context";
        this.quickContextDiv.style.position = 'fixed';

    };

    // Displays quick context help.
    this.showQuickContext = function () {
        var hint = this.hints[this.highlighted],
            help;

        if (hint) {
            if (hint.icon == 'icon-me-method') {
                help = hint.comment;
            }
        }

        if (help) {
            this.createQuickContextDiv(help);
            this.showQuickContextDiv();
            this.positionQuickContextDiv();
        } else {
            this.hideQuickContext();
        }
    };


    /*
    * Method context help methods.
    */

    // Method called from callback return function to fill and show the current context.
    this.fillContext = function (value) {
        this.context = value;

        if ((this.context == null) || (this.context == '')) {
            this.hideContext();
        } else {
            this.createContextDiv();
            this.showContextDiv();
        }
    };

    // Shows the method context.
    this.showContext = function (charCode) {
        if (!ascxMode && !this.isInsideComment()) {
            var currentLineMacro;
            if (this.isMixedMode) {
                // If the mode is mixed, parse only macro text and return only relevant parts of the code
                currentLineMacro = this.getCurrentLineMacro();
            } else {
                currentLineMacro = this.currentLineText() + '\n\n' + this.currentLinePos();
            }
            if (charCode == null) {
                this.callbackArgument = currentLineMacro + '\ncontext\n';
            } else {
                this.callbackArgument = currentLineMacro.replace('\n\n', '\n' + charCode + '\n') + '\ncontext\n';
            }
            this.callbackContext();
        }
    };

    // Hides the method context.
    this.hideContext = function () {
        this.hideContextDiv();
    };

    // Shows the hints div.
    this.showContextDiv = function () {
        this.contextDiv.style.display = 'block';
        this.positionDivs();
    };

    // Hides the hints div.
    this.hideContextDiv = function () {
        this.contextDiv.style.display = 'none';
        this.positionDivs(); // Position context hints when quick hints are closed
    };

    // Creates context div.
    this.createContextDiv = function () {

        this.contextDiv.innerHTML = this.context;
        this.contextDiv.className = "auto-complete-context";
        this.contextDiv.style.position = 'fixed';

    };

    /*
    * Hints methods.
    */

    // Highlights the first hint with the prefix of currently typed identifier
    this.findHint = function (lastChar, hideIfEmpty) {
        // Hide the hints if the line is empty, or we are at the end of the command
        if (hideIfEmpty) {
            var currentText = this.currentLineText();
            if ((currentText == '') || currentText.charAt(this.currentLinePos() - 1) == ';') {
                this.hideHints(true);
                return false;
            }
        }

        var identifier = this.locateIdentifier()[0];
        if (lastChar) {
            identifier += lastChar;
        }

        // Take identifier into account only if the dot was not pressed. 
        // This is workaround for code mirror delayed inserting of chars which may cause incorrect displaying of items
        if ((identifier != '') && (this.currentKeyCode != DOT_KEYCODE)) {
            identifier = identifier.toLowerCase();
            // Find the first item with given prefix
            var onlyMatch = true;
            var match = -1;

            var limitItems = (identifier.length > 2);

            // Represent search phrase: *identifier*
            var wildcardMatchCandidate = -1;

            // Clear displayed items array
            this.displayedItems = null;

            for (var i = 0; i < this.hints.length; i++) {
                // Get index
                var matchIndex = this.hints[i].name.toLowerCase().indexOf(identifier);

                if (matchIndex != -1) {

                    // Item starts with identifier => higher priority
                    if (matchIndex == 0) {
                        if (match != -1) {
                            // Keep information that we found more than one item
                            onlyMatch = false;

                            // Do not stop looping for limited items
                            if (!limitItems) {
                                break;
                            }
                        } else {
                            match = i;
                        }
                    }
                        // Item contains identifier
                    else if ((match == -1) && (wildcardMatchCandidate == -1)) {
                        wildcardMatchCandidate = i;
                    }

                    if (limitItems) {
                        if (this.displayedItems == null) {
                            this.displayedItems = [];
                        }
                        // Keep information that current element should be displayed
                        this.displayedItems.push(i);
                    }
                }
            }

            // Try use wildcard identifier
            if (match == -1) {
                match = wildcardMatchCandidate;
            }

            // Highlight matched hint
            this.highlighted = match;
            this.changeHighlight(true);

            return onlyMatch;
        } else {
            // If identifier is empty, highlight first item
            if (this.hints.length > 0) {
                this.highlighted = 0;
                this.changeHighlight(true);
            }
        }

        return false;
    };

    // Method called from callback return function to fill and show the current hints.
    this.fillHints = function (value) {
        this.hints = new Array();

        if ((value == '') && ascxMode) {
            return;
        }

        if (value != '') {
            try {
                this.hints = JSON.parse(value);
            } catch (e) {
                // Don't parse when error occurs
            }
        } else {
            // If we did not get any result there is no point showing hint when alphanumerical key is pressed
            this.showHintNextTime = false;
        }

        if (this.autoComplete) {
            this.autoComplete = false;
            if (this.findHint(null, false)) {
                // If only single or no hint found, use it
                this.useHint();
            } else {
                this.createHintsDiv();
                this.showHintsDiv();
                this.findHint(null, false);
            }
        } else if (this.hints.length > 0) {
            this.createHintsDiv();
            this.showHintsDiv();
            this.findHint(null, true);
        } else {
            this.hideHints();
        }
    };

    // Gets the current line number
    this.getLineNumber = function () {
        if (this.elem.lineNumber) {
            return this.elem.lineNumber(this.elem.cursorPosition().line);
        }
        else {
            return this.elem.getCursor().line;
        }
    };

    this.getLineContent = function (i) {
        if (this.elem.nthLine) {
            return this.elem.lineContent(this.elem.nthLine(i));
        }
        else {
            return this.elem.getLine(i);
        }
    };

    // Calls the callback function - ensures to show the hints.
    this.showHints = function (charCode) {
        if (ascxMode || !this.isInsideComment()) {
            if (ascxMode || !this.isMixedMode) {
                // If the mode is not mixed, pass current line and all the text before as parameters
                // If the mode is ascx, just pass current line text
                var prevText = '';
                var actLine = this.getLineNumber();
                for (var i = 1; i < actLine; i++) {
                    prevText += this.getLineContent();
                }

                if (charCode == null) {
                    this.callbackArgument = this.currentLineText() + '\n\n' + this.currentLinePos() + '\nhint\n' + (ascxMode ? '' : prevText);
                } else {
                    this.callbackArgument = this.currentLineText() + '\n' + charCode + '\n' + this.currentLinePos() + '\nhint\n' + (ascxMode ? '' : prevText);
                }
            } else {
                // If the mode is mixed, parse only macro text and return only relevant parts of the code

                // Locate part of current line which is macro text (might be whole line)
                var currentLineMacro = this.getCurrentLineMacro();

                // Parse all the macro parts before actual line
                var prevMacros = '';
                var lastPos = 0;
                var prevText = this.getTextToCaret(false);
                var brackets = 0;
                for (var i = 0; i < prevText.length - 2; i++) {
                    // If we are in mixed mode stop before we run to {%
                    if ((prevText.charAt(i) == '{') && (prevText.charAt(i + 1) == '%')) {
                        if (brackets == 0) {
                            lastPos = i + 2;
                        }
                        brackets++;
                    }
                    if ((prevText.charAt(i) == '%') && (prevText.charAt(i + 1) == '}')) {
                        brackets--;
                        // Append inside of a macro environment
                        if (brackets == 0) {
                            prevMacros += ";" + prevText.substring(lastPos, i);
                        }
                    }
                }

                // Append end of text if the environment is not finished
                if (brackets > 0) {
                    prevMacros += ";" + prevText.substring(lastPos, prevText.length - 1);
                }

                if (charCode == null) {
                    this.callbackArgument = currentLineMacro + '\nhint\n' + prevMacros;
                } else {
                    this.callbackArgument = currentLineMacro.replace('\n\n', '\n' + charCode + '\n') + '\nhint\n' + prevMacros;
                }
            }

            // Do the callback and fill the hints collection
            this.callbackHint();
        }
    };

    // Hide the hints.
    this.hideHints = function (nextTimeShow) {
        if (nextTimeShow != null) {
            this.showHintNextTime = nextTimeShow;
        }
        this.hideHintsDiv();
        this.highlighted = -1;
        this.hints = new Array();
        this.hideQuickContext();

        // When hints are not displayed ENTER and TAB should be handled normally, by editor
        this.elem.doNotHandleKeys = false;
    };

    this.replaceRange = function (newText, line, from, to) {
        if (this.elem.replaceRange) {
            var fromP = { line: line, ch: from };
            var toP = { line: line, ch: to };

            this.elem.replaceRange(newText, fromP, toP);
        }
        else {
            this.elem.selectLines(line, from, line, to);
            this.elem.replaceSelection(newText);
        }
    };

    // Uses the selected hint to code.
    this.useHint = function () {
        if ((this.hints.length > 0) && (this.highlighted > -1)) {

            var cursorIndex = -1;

            var hint = this.hints[this.highlighted];
            var nameToUse = hint.name;

            var offset = 0;
            if (nameToUse) {
                // Use code snippet when the tab was used to fire the hint usage
                if (hint.snippet) {
                    cursorIndex = hint.snippet.indexOf('|');
                    nameToUse = hint.snippet.replace('|', '');
                }
            }

            // Locate the current identifier
            var currentIdentifier = this.locateIdentifier();
            var pos1 = currentIdentifier[1];
            var pos2 = currentIdentifier[2];

            var currCursorLine = this.elem.cursorLine();

            var hidden = nameToUse.endsWith('%');
            if (hidden) {
                nameToUse = nameToUse.substring(0, nameToUse.length - 1);
            }

            if (nameToUse[0] == '[') {
                pos1 = pos1 - 1;
                this.replaceRange(nameToUse, currCursorLine, pos1, pos2);
            } else {
                this.replaceRange(nameToUse, currCursorLine, pos1, pos2);
            }

            var pos;
            if (cursorIndex == -1) {
                pos = { line: currCursorLine, ch: pos1 + nameToUse.length - offset };

            } else {
                pos = { line: currCursorLine, ch: pos1 + cursorIndex - offset };
            }
            this.elem.setCursor(pos);

            this.showHintAfterDotASCX = true;
        }
        this.hideHints(true);
    };

    // Shows the hints div.
    this.showHintsDiv = function () {
        if (this.hints.length > 0) {
            this.hintsDiv.style.display = 'block';

            this.highlighted = 0;
            this.changeHighlight(true);

            // When hints are displayed ENTER and TAB keys should not be handled by editor
            this.elem.doNotHandleKeys = true;

            this.positionDivs();
            this.showHintAfterDotASCX = true;
        }
    };

    // Hides the hints div.
    this.hideHintsDiv = function () {
        this.hintsDiv.style.display = 'none';
    };

    // Checks if the hints div is displayed
    this.isHintsDivDisplayed = function () {
        return this.hintsDiv.style.display == 'block';
    };

    // Changes the highlighted item in the hints div
    this.changeHighlight = function (adjustScrollBar) {

        var limitItems = (this.displayedItems != null);

        for (var i = 0; i < this.currentItems.length; i++) {
            var li = this.currentItems[i];
            if (li != null) {
                if (this.highlighted == i) {
                    li.className = "selected";
                }
                else {
                    if ((limitItems) && (this.displayedItems.indexOf(i) == -1)) {
                        li.className = "hidden";
                    }
                    else {
                        li.className = "";
                    }
                }
            }
        }

        // Line constants
        var lineHeight = 20;
        var maxAreaHeight = lineHeight * 9;

        this.showQuickContext();

        // Move scrollbar if required
        if (adjustScrollBar && this.highlighted != -1) {

            var scrollHeight = 0;
            var scrollTop = this.hintsDiv.scrollTop;

            // Compute current item position
            if (limitItems) {
                scrollHeight = lineHeight * this.displayedItems.indexOf(this.highlighted);
            }
            else {
                scrollHeight = lineHeight * this.highlighted;
            }


            // Move item only outside visible area
            if ((scrollHeight < scrollTop) || (scrollHeight > (scrollTop + maxAreaHeight))) {

                // Scroll down
                if (scrollHeight >= scrollTop + maxAreaHeight) {
                    this.hintsDiv.scrollTop = (scrollHeight - maxAreaHeight);
                }
                    // Scroll up
                else {
                    this.hintsDiv.scrollTop = scrollHeight;
                }
            }
        }

        // Move divs
        if (!this.isHintDown) {
            this.positionDivs(true);
        }
    };

    // Creates the hints div.
    this.createHintsDiv = function () {
        var ul = document.createElement('ul');

        this.displayedItems = null;
        this.currentItems = [];

        var lineMetaHintIndex = -1;

        // Create an array of LI's for the words.
        for (var i = 0; i < this.hints.length; i++) {
            var hint = this.hints[i];
            var li;
            var name = hint.name;

            if (name == '----') {
                lineMetaHintIndex = i;
                continue;
            }

            var type = hint.icon;
            if (type == null) {
                type = '';
            }

            // Set different style to hidden fields
            var hidden = name.endsWith('%');
            if (hidden) {
                name = name.substring(0, name.length - 1);
            }

            var img = document.createElement('i');
            var a = document.createElement('a');
            li = document.createElement('li');
            var textEnvelope = document.createElement('div');

            img.setAttribute("class", type);
            img.setAttribute("aria-hidden", "true");

            a.href = "javascript:";
            a.innerHTML = name;

            li.appendChild(img);
            li.appendChild(document.createTextNode(" "));
            li.appendChild(textEnvelope);
            textEnvelope.appendChild(a);

            if (hidden) {
                textEnvelope.className = "hidden-property";
            }

            if (me.highlighted == i) {
                var isMethod = (type == 'icon-method');
                li.className = "selected" + (isMethod ? " icon-method" : " icon-property");
            }

            if ((i > 0) && (this.hints[i - 1].name == '----')) {
                // Place line in this position
                li.style.borderTop = '1px solid';
            }

            ul.appendChild(li);

            this.currentItems.push(li);
        }

        // Get rid of line meta hint 
        if (lineMetaHintIndex > -1) {
            // Remove meta hint '----' from collection
            this.hints.splice(lineMetaHintIndex, 1);
        }

        this.hintsDiv.replaceChild(ul, this.hintsDiv.childNodes[0]);

        ul.onmouseup = function (ev) {
            // Walk up from target until you find the LI.
            var target = me.getEventSource(ev);
            while (target.parentNode && target.tagName.toLowerCase() != 'li') {
                target = target.parentNode;
            }

            me.highlighted = me.currentItems.indexOf(target);

            me.changeHighlight(false);
            me.cancelEvent(ev);
            me.hintsDiv.blur();
            me.elem.focus();
            return false;
        };

        ul.ondblclick = function (ev) {
            me.useHint();
            me.hideHints();
            me.cancelEvent(ev);
            me.hintsDiv.blur();
            me.elem.focus();
            return false;
        };

        this.hintsDiv.className = "auto-complete-hints";
        this.hintsDiv.style.position = 'fixed';

    };


    /*
    * Helper functions ensuring cross-browser functionality.
    */

    // Determines whether the cursor is inside a comment
    this.isInsideComment = function () {
        var prevText = this.getTextToCaret(true, true);
        var isLineComment = true;
        var isMultilineComment = true;
        for (var i = prevText.length - 1; i > 0; i--) {
            if ((prevText.charAt(i) == '\n') && isLineComment) {
                // If we run into new line first it cannot be inside an inline comment
                isLineComment = false;
            } else if ((prevText.charAt(i) == '/') && (prevText.charAt(i - 1) == '/') && isLineComment) {
                // If we run into // first it is inside an inline comment
                return true;
            } else if ((prevText.charAt(i) == '/') && (prevText.charAt(i - 1) == '*') && isMultilineComment) {
                // If we run into */ first it is not inside a comment
                isMultilineComment = false;
            } else if ((prevText.charAt(i) == '*') && (prevText.charAt(i - 1) == '/') && isMultilineComment) {
                // If we run into /* first it is inside a comment
                return true;
            }
        }
        return false;
    };

    this.positionDivsHorizontal = function (scroller, scrollerPos, caretPos) {
        // Get the maximum x position (autocomplete should not be outside the editor)
        var xMax = scrollerPos.x + scroller.offsetWidth - this.hintsDiv.offsetWidth,

            // Set horizontal position from caret position
            x = Math.min(caretPos.x - offset, xMax) - (this.leftOffset || 0);

        // Make sure left border of hints div is not more to the left than the left border of the editor
        if (x < scrollerPos.x) {
            x = scrollerPos.x;
        }

        return { hintsX: x, contextX: x };
    }

    this.setIsHintDown = function (scroller, scrollerPos, y) {
        // Make sure force above settings is applied
        var hintDown = !this.forceAbove;

        if (hintDown) {
            // Check that hints will fit in the document
            var autocompleteHeight = this.hintsDiv.offsetHeight,
                isAboveScreen,
                isBelowScreen;
            if (this.contextDiv.style.display != 'none') {
                autocompleteHeight += this.contextDiv.offsetHeight + offset;
            }

            isAboveScreen = y - offset - autocompleteHeight < 0;
            isBelowScreen = $cmsj(window).height() < y + 25 + autocompleteHeight;

            hintDown = ((y <= scrollerPos.y + scroller.offsetHeight / 2) && (!isBelowScreen || isAboveScreen)) || (isAboveScreen && !isBelowScreen);
        }

        this.isHintDown = hintDown;
    }

    this.positionDivsVertical = function (scroller, scrollerPos, initialY) {
        var y = initialY,
            contextY = y;

        if (this.isHintDown) {
            y += 25;

            contextY = y;

            if (this.contextDiv.style.display != 'none') {
                y += this.contextDiv.offsetHeight + offset;
            }
        } else {
            y -= offset;
            contextY = y;

            if (this.contextDiv.style.display != 'none') {
                y -= this.contextDiv.offsetHeight;
                contextY = y;

                y -= offset;
            }

            y -= this.hintsDiv.offsetHeight;
        }

        return { hintsY: y, contextY: contextY };
    }

    // Sets the correct position to a hints div.
    this.positionDivs = function (refreshOnlyVerticalPosition) {
        var scroller = this.elem.getScrollerElement(),

            // Get scroller and hints div offset parent position
            scrollerPos = this.getElementPosition(scroller),

            // Get absolute caret position
            caretPos = this.elem.cursorCoords(),

            x, y,

            // Subtracts the window scrollbar and converts to viewport coords.
            initialY = caretPos.y - $cmsj(window).scrollTop() - (this.topOffset || 0);

        if (!refreshOnlyVerticalPosition) {
            // Indicates whether to show quick context help on the left side
            // Do not flip side when the editor is too small
            this.quickContextLeft = (window.innerWidth - (caretPos.x + this.hintsDiv.offsetWidth) < 200) || (scroller.offsetWidth < 300);

            // Set if Hint is displayed below or above cursor
            this.setIsHintDown(scroller, scrollerPos, initialY);

            // Get hint and context horizontal position
            x = this.positionDivsHorizontal(scroller, scrollerPos, caretPos);
        }

        // Get hint and context vertical position
        y = this.positionDivsVertical(scroller, scrollerPos, initialY, refreshOnlyVerticalPosition);

        // Set context position
        if (this.isHintDown || this.contextDiv.style.display != 'none') {
            if (x) {
                this.contextDiv.style.left = x.contextX + unit;
            }
            this.contextDiv.style.top = y.contextY + unit;
        }

        // Set hint position
        if (x) {
            this.hintsDiv.style.left = x.hintsX + unit;
        }
        this.hintsDiv.style.top = y.hintsY + unit;

        // Set width of Context div
        if (window.innerWidth - caretPos.x < 300) {
            this.contextDiv.style.width = "300" + unit;
            this.contextDiv.style.right = "20" + unit;
            this.contextDiv.style.left = null;
            this.contextDiv.style.marginRight = null;
        }
        else {
            this.contextDiv.style.marginRight = "20" + unit;
            this.contextDiv.style.width = null;
        }

        this.positionQuickContextDiv();
    };


    this.positionQuickContextDiv = function () {
        var x, y, z, pos;

        // There is a bug in IE - getBoundingClientRect() throws "Unspecified error" after update panel update
        try {
            pos = this.hintsDiv.getBoundingClientRect();
        } catch (e) {
            pos = {
                top: this.hintsDiv.offsetTop,
                left: this.hintsDiv.offsetLeft,
                right: this.hintsDiv.offsetWidth + this.hintsDiv.offsetLeft,
                bottom: this.hintsDiv.offsetHeight + this.hintsDiv.offsetTop
            };
        }

        x = pos.left;
        y = pos.top;

        // Set width according to vertical scrollbar
        if (this.hasVerticalScrollbar()) {
            z = (window.innerWidth - (pos.right + offset)) - 36;
        } else {
            z = (window.innerWidth - (pos.right + offset)) - 20;
        }

        if (this.hintsDiv.style.display != 'none') {
            if (!this.quickContextLeft) {
                x += this.hintsDiv.offsetWidth + offset;
                this.quickContextDiv.style.width = z + unit;
            } else {
                x -= this.quickContextDiv.offsetWidth + offset;
                // Don't count width if it is on left
                this.quickContextDiv.style.width = '300' + unit;
            }
        }

        this.quickContextDiv.style.left = x + unit;
        this.quickContextDiv.style.top = y + unit;
    };

    // Check vertical scrollbar of the window/dialog
    this.hasVerticalScrollbar = function () {
        var scrollHeight = 0,
            $scrollElem;
        // E.g. web part properties window
        if (($scrollElem = $cmsj('.PageBody')[0]) != null) {
            scrollHeight = $scrollElem.scrollHeight;
        }
            // E.g. create new layout
        else if (($scrollElem = $cmsj('.PageContent')[0]) != null) {
            scrollHeight = $scrollElem.scrollHeight;
        }
            // E.g. field macro edit window
        else if (($scrollElem = $cmsj('.DialogPageBody')[0]) != null) {
            scrollHeight = $scrollElem.scrollHeight;
        }

        var bodyHeight = $cmsj('body')[0].clientHeight;
        var isVScrollbar = scrollHeight > bodyHeight;
        return isVScrollbar;
    };

    this.locateIdentifier = function (onlyPrefix) {
        var text = this.currentLineText();

        var pos1 = 0;

        for (var i = this.currentLinePos() - 1; i >= 0; i--) {
            var character = text.charAt(i);
            if ((character != '') && !character.match(ALPHANUMERIC_REGEX)) {
                pos1 = i + 1;
                break;
            }
        }
        var pos2;
        if (onlyPrefix) {
            pos2 = this.currentLinePos();
        } else {
            pos2 = text.length;
            for (var i = this.currentLinePos() ; i < text.length; i++) {
                var character = text.charAt(i);
                if ((character != '') && !character.match(ALPHANUMERIC_REGEX)) {
                    pos2 = i;
                    break;
                }
            }
        }
        return new Array(text.substring(pos1, pos2), pos1, pos2);
    };

    this.currentLinePos = function () {
        if (this.elem.cursorPosition) {
            return this.elem.cursorPosition().character;
        }
        else {
            return this.elem.getCursor().ch;
        }
    };

    this.currentLineText = function () {
        return this.elem.lineContent(this.elem.cursorLine());
    };

    this.autoCompleteEnabled = function () {
        // If it's not mixed mode, auto completion is always enabled
        if (ascxMode || !this.isMixedMode) {
            return true;
        } else {
            // In mixed mode, only if the caret is inside macro environment the auto completion is enabled
            // Get the whole text before actual line
            var text = this.getTextToCaret(true);
            for (var i = text.length; i > 0; i--) {
                var char1 = text.charAt(i);
                var char2 = text.charAt(i - 1);
                if ((char2 == '{') && (char1 == '%')) {
                    return true;
                } else if ((char2 == '%') && (char1 == '}')) {
                    return false;
                }
            }
            return false;
        }
    };

    // Returns whole text up to caret position (if includeCurrentLine is false, than it returns all the text from lines before current line).
    // If includeNewLineChars than \n is added inbetween lines
    this.getTextToCaret = function (includeCurrentLine, includeNewLineChars) {
        var text = '';
        var newLine = '';
        if (includeNewLineChars) {
            newLine = '\n';
        }
        var currLine = this.getLineNumber();;
        for (var i = 0; i < currLine; i++) {
            text += newLine + this.getLineContent(i);
        }
        if (includeCurrentLine) {
            text += newLine + this.currentLineText().substring(0, this.currentLinePos() + 1);
        }
        return text;
    };

    // Returns macro part around cursor of current line
    this.getCurrentLineMacro = function () {
        var caretPos = this.currentLinePos();
        var actLineText = this.currentLineText();
        var currentLineMacro = '';
        var pos1 = 0;
        var pos2 = actLineText.length;
        for (var i = this.currentLinePos() ; i > 0; i--) {
            // Stop when we run into {%
            if ((actLineText.charAt(i - 1) == '{') && (actLineText.charAt(i) == '%')) {
                pos1 = i + 1;
                caretPos -= i + 1;
                break;
            }
        }
        for (i = this.currentLinePos() ; i < actLineText.length - 1; i++) {
            // Stop when we run into %}
            if ((actLineText.charAt(i) == '%') && (actLineText.charAt(i + 1) == '}')) {
                pos2 = i;
                break;
            }
        }
        currentLineMacro = actLineText.substring(pos1, pos2);
        return currentLineMacro + '\n\n' + caretPos;
    };

    this.getCharCode = function (ev) {
        if (ev) {
            var isIE = (window.ActiveXObject) ? true : false;
            if (isIE) {
                return ev.keyCode;
            } else {
                return ev.charCode;
            }
        }
        if (window.event) {
            return window.event.keyCode;
        }
    };

    this.getKeyCode = function (ev) {
        if (ev) {
            return ev.keyCode;
        }
        if (window.event) {
            return window.event.keyCode;
        }
    };

    this.getEventSource = function (ev) {
        var e = window.event || ev;
        return e.srcElement || e.target;
    };

    this.cancelEvent = function (ev) {
        if (ev) {
            ev.stopPropagation();
            ev.preventDefault();
        }
        if (window.event) {
            window.event.returnValue = false;
        }
        return false;
    };

    // Gets the element position relative to the topmost offset parent.
    this.getElementPosition = function (e) {
        var position = { x: 0, y: 0 };
        while (e) {
            position.x += e.offsetLeft;
            position.y += e.offsetTop;
            e = e.offsetParent;
        }
        return position;
    };
}