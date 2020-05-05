cmsdefine(['CMS/EventHub', 'jQuery'], function (hub, $) {

    /**
     * Handles the keypress event for all the windows through raising the event hub 'KeyPressed' event
     */
    var handleOnKeyUp = function(e) {
        var evtObj = e ? e : window.event;

        // Handle CTRL + ALT + <key> combination on keyUp, because IE does not fire keydown for CTRL + ALT + Left/Right arrows
        return (evtObj.ctrlKey && evtObj.altKey);
    },

    keyUp = function (e) {
        if (handleOnKeyUp(e)) {
            keyPressed(e);
        }
    },

    keyDown = function (e) {
        if (!handleOnKeyUp(e)) {
            keyPressed(e);
        }
    },

    keyPressed = function (e) {
        var evtObj = e ? e : window.event;

        evtObj.key = e.charCode ? e.charCode : e.keyCode;
        
        hub.publish({ 
            name: 'KeyPressed',
            onlySubscribed: true,
            reversed: true,
            checkContinue: function(ev) {
                return !ev.wasHandled && !ev.notContinue;
            },
        }, evtObj);

        if (evtObj.wasHandled) {
            evtObj.keyCode = 0;
            evtObj.stopPropagation();
            evtObj.preventDefault();
            evtObj.returnValue = false;
            return false;
        }

        return true;
    },
        
    clicked = function(e, w) {
        // For now, emit the click only on topmost frame
        if (w.top.document !== w.document) {
            $(w.top.document).click();
        }

        var evtObj = e ? e : window.event;

        hub.publish({
            name: "GlobalClick",
            onlySubscribed: true,
            reversed: true,
            checkContinue: function(ev) {
                return !ev.wasHandled && !ev.notContinue;
            }
        }, evtObj);
    },
    
    GlobalEventsHandler = function () {
            // Handle global clicks through frames
            hub.subscribe('PageLoaded', function (a, w) {
                var $w = $(w);

                $w.click(function (e) {
                    clicked(e, w);
                })
                .keydown(keyDown)
                .keyup(keyUp);
            });
        };

    return GlobalEventsHandler;
});