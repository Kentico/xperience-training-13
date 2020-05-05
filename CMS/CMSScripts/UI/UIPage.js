
// Ensure using a popup dialog box for modal dialogs

// Initialize the UI page scripts
$cmsj(document).ready(function () {
    ContentResizer.Initialize();
});


// Content resizer - ensures that the content are will display a scroll bar when needed
var ContentResizer = {

    // Page elements
    Header: null,
    Content: null,
    Footer: null,

    // Variables
    resizeTimer: null,

    // Initializes the ContentResizer object
    Initialize: function () {

        // Initialize variables
        this.Header = $cmsj('.UIHeader');
        this.Content = $cmsj('.UIContent');
        this.Footer = $cmsj('.UIFooter');

        // Ensure correct displaying of the content area
        this.Content.css("height", "auto");

        // Resize the content area
        this.Resize();

        // Bind events which might cause a need for the content area height correction
        this.Header.bind("DOMSubtreeModified", function () {
            ContentResizer.DelayedResize();
        });
        $cmsj(window).resize(function () {
            ContentResizer.DelayedResize();
        });

        // Manually invoke Resize because some dialogs do not adjust their size on "DOM.ready" event but sooner
        $cmsj(window).resize();
    },

    // Invoke delayed content resize (avoids multiple simultaneous calls)
    DelayedResize: function () {
        clearInterval(this.resizeTimer);
        this.resizeTimer = setTimeout(function () {
            ContentResizer.Resize();
        }, 50);
    },

    // Resize the content area
    Resize: function () {
        this.Content
            .css("top", this.Header.height() || 0)
            .css("bottom", this.Footer.height() || 0);
    }
};
