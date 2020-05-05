var ChatNotificationManager = (function () {
    var my = {},
        titles = [],
        events = [],
        win = $cmsj(window),
        tabTitleBackup = '',
        tabBlinkTimeout = null,
        isSoundReady = false,
        soundError = false,
        hasFocus = true;

    my.Notify = function (eventName, force) {
        if (events[eventName] == null) {
            return;
        }
        if (force || !hasFocus) {
            PlaySound(eventName);
            TabBlinkStart(eventName);
        }
    };

    my.NotifySound = function (eventName, force) {
        if (events[eventName] == null) {
            return;
        }
        if (force || !hasFocus) {
            PlaySound(eventName);
        }
    };

    my.NotifyTab = function (eventName, force) {
        if (events[eventName] == null) {
            return;
        }
        if (force || !hasFocus) {
            TabBlinkStart(eventName);
        }
    };

    my.StopNotify = function (eventName) {
        if (events[eventName] == null) {
            return;
        }
        TabBlinkStop();
    };

    my.AddEventNotification = function (eventName, soundUrl, tabTitle) {
        if (events[eventName] != null) {
            return;
        }

        if (!isSoundReady && !soundError) {
            setTimeout(function () {
                window.ChatNotificationManager.AddEventNotification(eventName, soundUrl, tabTitle);
            }, 500);
            return;
        }

        if ((soundUrl != null) && (soundUrl.length > 0) && !soundError) {
            soundManager.createSound({
                id: eventName,
                url: soundUrl
            });
        }
        titles[eventName] = tabTitle;
        events[eventName] = true;
    };

    function Initialize() {
        win.hasFocus = true;
        soundManager.setup({
            url: window.ChatNotificationManagerSettings.SwfFolder,
            debugMode: false,
            preferFlash: false,
            onready: function () {
                isSoundReady = true;
            },
            ontimeout: function () {
                soundError = true;
                if (typeof (console) !== "undefined") {
                    console.log("Error occured while initializing Chat sound manager.");
                }
            }
        });

        win.focus(function () {
            hasFocus = true;
            TabBlinkStop();
        });

        win.blur(function () {
            hasFocus = false;
        });
    }

    function TabBlink(newTitle) {
        if (hasFocus) {
            TabBlinkStop();
            return;
        }

        var title = window.top.document.title;
        if (title == newTitle) {
            window.top.document.title = tabTitleBackup;
        } else {
            tabTitleBackup = title;
            window.top.document.title = newTitle;
        }
        tabBlinkTimeout = setTimeout(function () { TabBlink(newTitle); }, 2000);
    }

    function TabBlinkStart(eventName) {
        if (tabBlinkTimeout == null) {
            var title = titles[eventName];
            if ((title != null) && (title.length > 0)) {
                TabBlink(title);
            }
        }
    }

    function TabBlinkStop() {
        if (tabBlinkTimeout) {
            clearTimeout(tabBlinkTimeout);
            tabBlinkTimeout = null;
            if (tabTitleBackup) {
                window.top.document.title = tabTitleBackup;
            }
        }
    }

    function PlaySound(eventName) {
        if (!isSoundReady) {
            return;
        }
        var sound = soundManager.getSoundById(eventName);
        if (sound != null) {
            sound.play();
        }
    }

    Initialize();

    return my;
}());


function ChatNotificationManagerSetup(opt) {
    window.ChatNotificationManager.AddEventNotification(opt.eventName, opt.soundFile, opt.notifyTitle);
}