/** 
 * Confirm dialog utility class. Provides API for displaying confirmation dialog when navigating from some long-running task.
 */
cmsdefine(['CMS/EventHub'], function(EventHub) {

    var globalCheckChanges = window.CheckChanges,
        NavigationBlocker = function() {
            this.isBlocked = false;
            this.message = undefined;
            this._bindToWindowUnload();

            EventHub.subscribe("NavigationBlocker.Block", this._block.bind(this));
            EventHub.subscribe("NavigationBlocker.Unblock", this._unblock.bind(this));
        };

    
    /**
     * Checks if there are some unsaved changes in the UI or the UI is blocked by long-running task. 
     * @return  {boolean}  True, if the UI is not blocked, otherwise false
     */
    NavigationBlocker.prototype.canNavigate = function() {
        var result = true;

        if (globalCheckChanges) {
            result = globalCheckChanges();
        }

        if (this.isBlocked) {
            result = confirm(this.message);
            if (result) {
                this.unblock();
            }
        }

        return result;
    };


    /**
     * Ensures that until the unblock method is called, canNavigate method will return false.
     * @param  {string}  message  message to be displayed in the confirmation dialog
     */
    NavigationBlocker.prototype.block = function(message) {
        EventHub.publish({ name: "NavigationBlocker.Block", onlySubscribed: true }, message);
    };


    /** 
     * Unblocks navigation.
     */
    NavigationBlocker.prototype.unblock = function() {
        EventHub.publish({ name: "NavigationBlocker.Unblock", onlySubscribed: true });
    };
    

    /**
     * Ensures that until the unblock method is called, canNavigate method will return false.
     * @param  {string}  message  message to be displayed in the confirmation dialog
     */
    NavigationBlocker.prototype._block = function(message) {
        window.disableTabCallback = true;
        this.message = message;
        this.isBlocked = true;
    };


    /**
     * Unblocks navigation.
     */
    NavigationBlocker.prototype._unblock = function() {
        this.isBlocked = false;
        this.message = undefined;
        window.disableTabCallback = false;
    };


    /**
     * Binds to the window unload event. Shows confirmation message if any blocker is registered.
     */
    NavigationBlocker.prototype._bindToWindowUnload = function() {
        var that = this;

        window.onbeforeunload = function() {
            if (that.isBlocked) {
                return that.message;
            }
        };
    };
    
    return NavigationBlocker;
});