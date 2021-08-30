
// Checks whether the page content has been changed. If it has, displays a confirmation message.
function CheckChanges() {
    return CMSContentManager.checkChanges();
}

// Sets the page content changed state
function Changed() {
    CMSContentManager.changed();
}

// Object which handles functionality about page content changes.
var CMSContentManager = {
    // Variables
    allowSubmit: false,
    confirmLeave: null,
    confirmLeaveShort: null,
    confirmChanges: true,
    hdnSaveChangesId: 'saveChanges',
    hdnContentChangedId: 'contentChanged',
    _isContentChanged: null,
    checkChangedFields: null,
    eventManager: null,
    fullFormScope: true,
   

    // Functions

    // Extend the postback url with the information about changed page content to ensure keeping this information through server page redirects (i.e. "URLHelper.Redirect(CurrentURL);")
    storeContentChangedStatus: function () {
        var jForm = $cmsj('form');
        var postbackUrl = jForm.attr("action");
        postbackUrl = this.storeContentChangedInUrl(postbackUrl);
        jForm.attr("action", postbackUrl);
    },

    // Extends the given url with the information indicating whether the page content has been changed
    storeContentChangedInUrl: function (url) {
        if ($cmsj.param && $cmsj.param.querystring) {
            url = $cmsj.param.querystring(url, "cmscontentchanged=" + this._contentChanged());
            if (!this._contentChanged()) {
                // Remove cmscontentchanged from URL and preserve & or ? correctly
                url = url.replace(/((&)|(\?))cmscontentchanged=([^&]*)(&)?/gi, '$3$2');
            }
        }

        return url;
    },

    allowChanged: function (elementId) {
        if (this.checkChangedFields == null) {
            return true;
        }

        for (var i = 0; i < this.checkChangedFields.length; i++) {
            if (this.checkChangedFields[i] == elementId) {
                return true;
            }
        }
        return false;
    },

    // Changes the status which indicating whether the page content has been changed
    changed: function (isModified) {
        if (typeof (isModified) === "undefined") {
            isModified = true;
        }

        this._setContentChanged(isModified);

        // Raise 'contentChanged' event
        this.eventManager.trigger('contentChanged', isModified);
    },

    // Checks whether the page content has been changed. If it has, displays a confirmation message.
    checkChanges: function () {
        if (this.contentModified() && (this.confirmLeave != null) && this.confirmChanges) {
            if (confirm(this.confirmLeave)) {
                this.changed(false);
                this._resetEditorsIsDirty();
                refreshPageOnClose = true;
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return true;
        }
    },

    submitAction: function () {
        if (this.contentModified() && !this.allowSubmit) {
            $cmsj('#' + this.hdnSaveChangesId).val('1');
        }
        else {
            $cmsj('#' + this.hdnSaveChangesId).val('0');
        }

        this.allowSubmit = true;
        return true;
    },

    submitPage: function () {
        return (!this.contentModified() || this.allowSubmit || this.submitAction());
    },

    // Initializes the CMSContentManager object
    initChanges: function () {

        // Get elements search condition with dependence on current settings
        var searchScope = 'form *';
        if (!this.fullFormScope) {
            // Select element designed with special data attribute and all its children
            searchScope = "[data-tracksavechanges='true'], [data-tracksavechanges='true'] *";
        }

        // Get elements
        var formChildren = $cmsj(searchScope)
            .filter(":input")
            .not(".dont-check-changes, .dont-check-changes *")
            .not(":input[type=image]");

        // Bind 'change' event to the input elements
        formChildren.each(function (index) {
            var elem = $cmsj(this);
            if (CMSContentManager.allowChanged(elem.attr("id"))) {
                elem.bind('change paste', function (event) {
                    CMSContentManager.fieldChanged(event.target);
                });

                if (!elem.is('[readonly]')) {
                    elem.bind('keyup', function (event) {
                        CMSContentManager.fieldChanged(event.target);
                    });
                }
            }
        });

        // Parse query string
        if ($cmsj.deparam && $cmsj.deparam.querystring) {
            var queryObj = $cmsj.deparam.querystring(location.href, true);

            if (this._isContentChanged == null) {
                this.changed(false);

                // Try to restore the '_contentChanged' variable from url
                if (queryObj['cmscontentchanged'] != null) {
                    this.changed(queryObj.cmscontentchanged);
                }
            }
        }
    },

    // Returns a value indication whether the page content has been changed.
    contentModified: function () {
        return this._contentChanged() || this._editorsChanged();
    },

    _fullTrim: function (text) {
        return text.replace(/\s+/g, "");
    },

    _contentChanged: function () {
        return $cmsj('#' + this.hdnContentChangedId).val() == "true";
    },

    _setContentChanged: function (val) {
        $cmsj('#' + this.hdnContentChangedId).val(val);
        this._isContentChanged = val;
    },
    
    _editorsChanged: function () {
        try {
            var ckEditor = window.CKEDITOR;
            if ((typeof (ckEditor) != 'undefined') && (ckEditor.instances != null)) {
                for (var name in ckEditor.instances) {
                    if (this.allowChanged(name)) {
                        var oEditor = ckEditor.instances[name];
                        if (oEditor.checkDirty()) {
                            var prevValue = oEditor._.previousValue;
                            var oldText = this._fullTrim(prevValue);

                            var newText = this._fullTrim(oEditor.getData());
                            if (oldText !== newText) {
                                return true;
                            }
                        } else {
                            oldText = this._fullTrim(document.getElementById(name).innerText);
                            newText = this._fullTrim(oEditor.getData());
                            if (oldText !== newText) {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        catch (ex) {
        }
        return false;
    },

    _resetEditorsIsDirty: function () {
        try {
            var ckEditor = window.CKEDITOR;
            if ((typeof (ckEditor) != 'undefined') && (ckEditor.instances != null)) {
                for (var name in ckEditor.instances) {
                    if (this.allowChanged(name)) {
                        var oEditor = ckEditor.instances[name];
                        if (oEditor.checkDirty()) {
                            oEditor.resetDirty();
                        }
                    }
                }
            }
        }
        catch (ex) {
        }
        return false;
    },

    isTopFrameValid: function() {
        return (top.frames['cmsdesktop'] != null);
    },

    // Sets 'changed' status of the page with respect to 'data-ignorechanges' attribute.
    fieldChanged: function (element) {
        if (element) {
            if (!$cmsj(element).data('ignorechanges')) {
                CMSContentManager.changed();
            }
        }
    }
};


// Register the CMSContentManager event manager object
if (CMSContentManager.eventManager == null) {
    CMSContentManager.eventManager = $cmsj("<div id=\"cmsCMEventManager\" style=\"display: none\"></div>");
    $cmsj("body").append(CMSContentManager.eventManager);
}

$cmsj(document).ready(function () {
    CMSContentManager.initChanges();

    $cmsj('form').submit(function () {
        return CMSContentManager.submitPage();
    });
});