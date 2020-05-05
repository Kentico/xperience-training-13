/**
*	UI Layout Callbacks Package
*
*	NOTE: These callbacks must load AFTER the jquery.layout...js library loads
*
*	Updated:	2011-07-10
*	Author:		Kevin Dalman (kevin.dalman@gmail.com)
*/
; (function ($) {
    var _ = $.layout; // make sure the callbacks branch exists
    if (!_.callbacks) _.callbacks = {};

    // make sure $.layout.defaults exists (backward compatibility
    if (!_.defaults) _.defaults = { north: {}, south: {}, east: {}, west: {}, center: {} };


    /**
    *	UI Layout Callback: resizePaneAccordions
    *
    *	This callback is used when a layout-pane contains 1 or more accordions
    *	- whether the accordion a child of the pane or is nested within other elements
    *	Assign this callback to the pane.onresize event:
    *
    *	SAMPLE:
    *	$cmsj("#elem").tabs({ show: $.layout.callbacks.resizePaneAccordions });
    *	$cmsj("body").layout({ center__onresize: $.layout.callbacks.resizePaneAccordions });
    *
    *	Version:	1.0 - 2011-07-10
    *	Author:		Kevin Dalman (kevin.dalman@gmail.com)
    */
    _.callbacks.resizePaneAccordions = function (x, ui) {
        // may be called EITHER from layout-pane.onresize OR tabs.show
        var $P = ui.jquery ? ui : $cmsj(ui.panel);
        // find all VISIBLE accordions inside this pane and resize them
        $P.find(".ui-accordion:visible").each(function () {
            var $E = $cmsj(this);
            if ($E.data("accordion"))
                $E.accordion("resize");
        });
    };


    /**
    *	UI Layout Callback: resizeTabLayout
    *
    *	Requires Layout 1.3.0.rc29.15 or later
    *
    *	This callback is used when a tab-panel is the container for a layout
    *	The tab-layout can be initialized either before or after the tabs are created
    *	Assign this callback to the tabs.show event:
    *	- if the layout HAS been fully initialized already, it will be resized
    *	- if the layout has NOT fully initialized, it will attempt to do so
    *		- if it cannot initialize, it will try again next time the tab is accessed
    *		- it also looks for ANY visible layout *inside* teh tab and resize/init it
    *
    *	SAMPLE:
    *	$cmsj("#elem").tabs({ show: $.layout.callbacks.resizeTabLayout });
    *	$cmsj("body").layout({ center__onresize: $.layout.callbacks.resizeTabLayout });
    *
    *	Version:	1.1 - 2011-07-05
    *	Author:		Kevin Dalman (kevin.dalman@gmail.com)
    */
    _.callbacks.resizeTabLayout = function (x, ui) {
        // may be called EITHER from layout-pane.onresize OR tabs.show
        var $P = ui.jquery ? ui : $cmsj(ui.panel);
        // find all VISIBLE layouts inside this pane/panel and resize them
        $P.filter(":visible").find(".ui-layout-container:visible").addBack().each(function () {
            var layout = $cmsj(this).data("layout");
            if (layout) layout.resizeAll();
        });
    };


    /**
    *	UI Layout Callback: pseudoClose
    *
    *	Prevent panes from closing completely so that an iframes/objects 
    *	does not reload/refresh when pane 'opens' again.
    *	This callback preventing a normal 'close' and instead resizes the pane as small as possible
    *
    *	SAMPLE:
    *	pseudoClose:	{ selector: "#myObject" }
    *	south__onclose:	$.layout.callbacks.pseudoClose
    *
    *	Version:	1.1 - 2011-07-10
    *	Author:		Kevin Dalman (kevin.dalman@gmail.com)
    */
    // init default pseudoClose-options when library loads
    for (var i = 0; i < 4; i++) {
        _.defaults[["north", "south", "east", "west"][i]].pseudoClose = {
            hideObject: "iframe" // find and hide this when 'closed' - usually: "", "pane", "iframe" or "object"
	, skipIE: false	// can skip IE for iframes that do not contain media objects
        };
    };
    _.callbacks.pseudoClose = function (pane, $Pane, paneState, paneOptions) {
        var fN = "pseudoClose"
	, o = paneOptions
	, oFn = $.extend({}, $.layout.defaults[pane][fN], o[fN]) // COPY the pseudoClose options
	;
        if (oFn.skipIE && $.layout.browser.msie) return true; // ALLOW close
        if (oFn.hideObject === "object") oFn.hideObject += ",embed"; // 'embedded objects' are often <EMBED> tags

        setTimeout(function () {
            var sel = oFn.hideObject
		, $Obj = sel === "pane" || $Pane[0].tagName === sel.toUpperCase() ? $Pane : $Pane.find(sel)
		, layout = $Pane.data("parentLayout")
		, s = layout.state[pane]	// TEMP until paneState is *no longer* a 'copy' (RC29.15)
		, d = s[fN] || {}
		, siz = 'size'
		, min = 'minSize'
		, rsz = "resizable"
		, vis = "visibility"
		, v = "visible"
		, h = "hidden"
		;
            if (d[siz]) {
                if (d[rsz]) layout.enableResizable(pane); // RE-ENABLE manual-resizing
                o[min] = d[min]; 			// RESET minSize option
                layout.setSizeLimits(pane); 	// REFRESH state.minSize with new option
                layout.sizePane(pane, d[siz]); // RESET to last-size
                d = {}; 						// CLEAR data logic
                $Obj.css(vis, h).css(vis, v); 	// fix visibility bug
            }
            else {
                d[siz] = s[siz]; 			// SAVE current-size
                d[min] = o[min]; 			// ditto
                o[min] = 0; 					// SET option so pane shrinks as small as possible
                d[rsz] = o[rsz]; 			// SAVE resizable option
                layout.disableResizable(pane); // DISABLE manual-resizing while pseudo-closed
                layout.setSizeLimits(pane); 	// REFRESH state.minSize with new option
                layout.sizePane(pane, s[min]); // SIZE to minimum-size
                $Obj.css(vis, h); 			// HIDE pane or object (only if hideObject is set & exists)
            }
            s[fN] = d; // save data
        }, 50);

        return false; // CANCEL normal 'close'
    };


})($cmsj);