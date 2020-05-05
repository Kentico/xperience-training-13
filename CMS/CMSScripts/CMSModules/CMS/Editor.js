cmsdefine(['CMS/EventHub'], function (hub) {
    var keyPressed = function (e) {
        var evtObj = e ? e : window.event;
        if (!evtObj.key) {
            evtObj.key = e.charCode ? e.charCode : e.keyCode;
        }

        hub.publish({
            name: 'KeyPressed',
            onlySubscribed: true,
            reversed: true,
            checkContinue: function (ev) {
                return !ev.wasHandled;
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

    HtmlEditor = function () {
        window.HtmlEditor = this;

        this.keyPressed = keyPressed;
    };

    return HtmlEditor;
});