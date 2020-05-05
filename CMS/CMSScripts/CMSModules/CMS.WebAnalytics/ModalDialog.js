cmsdefine(["jQuery", "Underscore"], function ($, _) {
    
    var defaults = {
        width: 500,
        height: 500,
        title: "Default title",
        draggable: true,
        closeButton: true,
        canBeMaximized: true,
        onClose: function() {},
        resourceStrings: {
            maximize: "Full screen",
            restore: "Restore",
            close: "Close",
        },
        buttons: []
    };


    /**
     * Wrapper around the jQuery UI dialog widget, respecting the CMS design guidelines. Should be used in case when
     * the content to be displayed is part of the DOM, not the whole page which should be displayed in iframe. For such
     * cases there is modalDialog method in global space available.
     *
     * @param   contentSelector         string      selector of the DOM object to be displayed within the dialog
     * @param   settings                object      settings object for configuring the dialog behavior and appearance
     *              width               int         specifies width of the dialog in pixels
     *              height              int         specifies height of the dialog in pixels
     *              title               string      specifies title of the dialog
     *              draggable           boolean     determines whether the dialog should be draggable or not. Dialog title bar is used for handle
     *              closeButton         boolean     determines whether the close button should be available within the dialog title bar
     *              canBeMaximized      boolean     determines whether the dialog can be maximized to fill the screen and then restored to its original size and position
     *              onClose             callback    method called when the dialog is closed
     *              resourceStrings     object      object containing localization strings used by the dialog
     *                  maximize        string      maximize button title
     *                  restore         string      restore button title
     *                  close           string      close button title
     *              buttons             array       collection of buttons to be displayed in the dialog footer. Every button is represented by the object with properties
     *                                              'title', specifying text inside the button and 'action', containing method called whenever the button is clicked
     * 
     * @example
     * Following example shows how to use the dialog with some common settings
     * 
     * caller.html
     * <div id="dialog-content" style="display:none">
     *   <h1>Some header</h1>
     *   ... other content
     * </div>
     *
     * <button id="btn">Show dialog</button>
     *
     * caller.js
     * var dialog = new ModalDialog("#dialog-content", { title: "My title", buttons: [{ title: "Enter", action: function () { alert("Enter clicked"); } }] });
     *
     * $("#btn").on("click", function() {
     *     dialog.open();
     * })
     */
    var ModalDialog = function (contentSelector, settings) {
        if (!contentSelector) {
            throw "Content selector has to be specified";
        }
        
        this._settings = _.defaults(settings || {}, defaults);

        var $dialog = this._buildDialog(),
            $topDialog = this._moveToTopFrameContext($dialog);

        this._$dialogContent = $(contentSelector);
        this._$dialogContentParent = this._$dialogContent.parent();
        
        this._isMaximized = false;
        
        if (!$topDialog.dialog) {
            console.error("jQuery UI dialog widget is not available.");
            return;
        }

        this._dialog = $topDialog.dialog({
            autoOpen: false,
            width: this._settings.width,
            height: this._settings.height,
            title: this._settings.title,
            modal: true,
            draggable: this._settings.draggable,
            close: this._settings.onClose
        });

        this._setupWidget();
        this._cleanUp();
        this._handleWindowResize();
    };


    /**
     * Opens the dialog.
     */
    ModalDialog.prototype.open = function () {
        this._$content.append(this._$dialogContent.show());
        this._dialog.dialog("open");
    };


    /**
     * Closes the dialog. 
     */
    ModalDialog.prototype.close = function () {
        this._dialog.dialog("close");
        this._$dialogContentParent.append(this._$dialogContent.hide());
    };


    /**
     * Fires on the window resize. In case the dialog is in maximized state, adjusts its size and ensures the dialog position in the center of the window.
     */
    ModalDialog.prototype._handleWindowResize = function() {
        $(window.top).resize(function () {
            if (this._isMaximized) {
                this._resizeToMaximalSize();
                this._center();
            }
        }.bind(this));
    };


    /**
     * Sets up the dialog underlying jQuery UI widget. Adds custom CSS class and ensure the dialog header will be the drag handle.
     */
    ModalDialog.prototype._setupWidget = function() {
        this._widget = this._dialog.dialog("widget");
        this._widget.addClass("modal-dialog");

        if (this._settings.draggable) {
            this._setupWidgetDraggable();
        }
    };    


    /**
     * Sets up the dialog underlying jQuery UI widget draggable environment. Ensures the dialog header bar will be the drag handle.
     */
    ModalDialog.prototype._setupWidgetDraggable = function() {
            this._widget.draggable({
                cancel: '.ui-dialog-titlebar-close',
                handle: '.dialog-header',
            });
    };


    /**
     * Depending on the current state, either maximize or restore the dialog.
     */
    ModalDialog.prototype._toggleSize = function() {
        if (this._isMaximized) {
            this._restore();
        } else {
            this._maximize();
        }
    };


    /**
     * Moves the dialog to the top most frame. Uses jQuery context of the top most frame
     * to make sure all interactions will be working.
     *
     * @param  dialog  DOM object       dialog object to be injected to the top most frame.
     *
     * @returns        jQuery object    jQuery object representing the dialog existing in the top most frame context
     */
    ModalDialog.prototype._moveToTopFrameContext = function(dialog) {
        $(window.top.document).append(dialog);
        return window.top.$cmsj.noConflict()(dialog);
    };


    /**
     * Build DOM object representing the dialog with all its parts, including header, content placeholder and the footer.
     * Hooks event handlers to the header buttons.
     */
    ModalDialog.prototype._buildDialog = function() {
        var dialogRoot = $("<div></div>"),
            dialogHeader = $("<div />")
                .addClass("dialog-header non-selectable"),
            actionButtons = $("<div />")
                .addClass("dialog-header-action-buttons"),
            title = $("<h2 />")
                .addClass("dialog-header-title")
                .text(this._settings.title),
            content = $("<div />")
                .addClass("dialog-content"),
            footer = $("<div />").addClass("dialog-footer control-group-inline");

        dialogHeader.on('dblclick', this._toggleSize.bind(this));

        if (this._settings.canBeMaximized) {
            this._toggleButton = this._buildActionButton("action-button", "icon-modal-maximize cms-icon-80", this._settings.resourceStrings.maximize, "toggle");
            this._toggleButton.find("i").on("click", this._toggleSize.bind(this));
            actionButtons.append(this._toggleButton);
        }

        if (this._settings.closeButton) {
            var closeButton = this._buildActionButton("action-button close-button", "icon-modal-close cms-icon-150", this._settings.resourceStrings.close, "close");
            closeButton.find("i").on("click", this.close.bind(this));
            actionButtons.append(closeButton);
        }

        if (this._settings.draggable) {
            dialogHeader.addClass("handle");
        }

        dialogHeader.append(actionButtons).append(title);
        dialogRoot.append(dialogHeader).append(content).append(footer);

        this._$dialogHeader = dialogHeader;
        this._addButtons(footer);
        this._$content = content;

        return dialogRoot;
    };


    /**
     * Adds all buttons provided with the input settings object to the dialog footer. Registers their click events to the callbacks 
     * provided in the configuration object.
     *
     * @param    dialogFooter    jQuery object    represents the dialog footer
     */
    ModalDialog.prototype._addButtons = function(dialogFooter) {
        this._settings.buttons.forEach(function (button) {
            dialogFooter.append($("<button />").addClass("btn btn-primary").text(button.title).on("click", button.action));
        });
    };


    /**
     * Builds DOM object representing action button in the dialog header.
     * 
     * @param   buttonClass     string      CSS class to be added to the outer most div element of the button. Usually contains 'action-button'
     * @param   iconClass       string      CSS class defining font icon of the button. Should contain both icon type and size
     * @param   title           string      used as tooltip of the button
     *
     * @returns                             DOM object containing the built button
     */
    ModalDialog.prototype._buildActionButton = function (buttonClass, iconClass, title) {
        return $("<div />")
            .addClass(buttonClass)
            .append("<a/>")
            .find("a")
            .append(
                $("<span />")
                    .addClass("sr-only")
                    .text(title)
            )
            .append(
                $("<i />")
                    .addClass(iconClass)
                    .attr("aria-hidden", "true")
                    .css("cursor", "pointer")
                    .attr("title", title)
            )
            .end();
    };


    /**
     * Performs maximization of the dialog. Stores original size and position for restoring the dialog in future. 
     * After dialog maximization moves it to the center of the window.
     */
    ModalDialog.prototype._maximize = function () {
        this._originalSize = {
            width: this._dialog.dialog("option", "width"),
            height: this._dialog.dialog("option", "height"),
        };
        this._originalPosition = this._dialog.dialog("option", "position");

        this._dialog.dialog("option", "draggable", false);
        this._dialog.dialog("option", "resizable", false);
        this._$dialogHeader.removeClass("handle");

        this._changeToggleButtonToRestore();

        this._resizeToMaximalSize();
        this._center();
        this._isMaximized = true;
    };


    /**
     * Changes maximize button to the restore one. Alters both icon and the tooltip text.
     */
    ModalDialog.prototype._changeToggleButtonToRestore = function() {
        this._toggleButton.find("i").removeClass("icon-modal-maximize").addClass("icon-modal-minimize").attr("title", this._settings.resourceStrings.restore);
        this._toggleButton.find("a span").text(this._settings.resourceStrings.restore);
    };
    

    /**
    * Changes restore button to the maximize one. Alters both icon and the tooltip text.
    */
    ModalDialog.prototype._changeToggleButtonToMaximize = function() {
        this._toggleButton.find("i").removeClass("icon-modal-minimize").addClass("icon-modal-maximize").attr("title", this._settings.resourceStrings.maximize);
        this._toggleButton.find("a span").text(this._settings.resourceStrings.maximize);
    };


    /**
     * Performs restore (minimize) action of the dialog. Resize the dialog to the previously saved dimensions and move the dialog to the saved position.
     */
    ModalDialog.prototype._restore = function() {
        this._dialog.dialog("option", "width", this._originalSize.width);
        this._dialog.dialog("option", "height", this._originalSize.height);
        this._dialog.dialog("option", "position", this._originalPosition);

        this._dialog.dialog("option", "draggable", this._settings.draggable);
        this._dialog.dialog("option", "resizable", true);

        this._changeToggleButtonToMaximize();
        this._setupWidgetDraggable();
        this._$dialogHeader.addClass("handle");

        this._isMaximized = false;
    };


    /**
     * Maximize the dialog to its maximal size, respecting the margin.
     */
    ModalDialog.prototype._resizeToMaximalSize = function() {
        var windowWidth = $(window.top).width(),
            windowHeight = $(window.top).height(),
            verticalPadding = 24,
            horizontalPadding = 48;

        this._dialog.dialog("option", "width", windowWidth - 2 * horizontalPadding);
        this._dialog.dialog("option", "height", windowHeight - 2 * verticalPadding);
    };


    /**
     * Move dialog to the center of the window.
     */
    ModalDialog.prototype._center = function() {
        this._dialog.dialog( "option", "position", { my: "center", at: "center", of: window.top } );
    };


    /**
     * Removes unnecessary DOM object generated by the jQuery UI dialog.
     */
    ModalDialog.prototype._cleanUp = function () {
        // This ID is used in aria-labelledby attribute of the dialog. Since the original title is removed, 
        // the ID has to be moved to the new dialog header to satisfy accessibility of the dialog
        var widgetTitleBar = this._widget.find(".ui-dialog-titlebar"),
            ariaLabelledById = widgetTitleBar.find("*[id]").attr("id");
        this._$dialogHeader.attr("id", ariaLabelledById);
        
        widgetTitleBar.remove();
    };

    return ModalDialog;
});