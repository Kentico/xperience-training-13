/** 
 * Mole module for traversing frames
 */
cmsdefine(['CMS/EventHub', 'CMS/Application', 'Underscore'], function (hub, application) {
    var traverseTimeout = null,

        traverseFrames = function (w, level, output) {
            var i = 0,
                framesLength = w.frames.length,
                app;

            if (framesLength > 0) {
                output[level] = output[level] || [];

                for (i; i < framesLength; i++) {
                    var fr = w.frames[i];

                    if (!fr) {
                        continue;
                    }

                    // Check if the frame domain is same as my,
                    // do not traverse foreign frames
                    try {
                        var url = fr.location.href;
                    } catch (e) {
                        continue;
                    }

                    app = application.getData(null, fr);
                    if (app && !app.isDialog) {
                        app.frame = fr;
                        output[level].push(app);

                        traverseFrames(fr, level + 1, output);
                    } else if (!app) {
                        traverseFrames(fr, level + 1, output);
                    }
                }
            }
        },

        onTraverseTimeout = function (args) {
            var data = [],
                myArgs = _.toArray(args);

            traverseTimeout = null;
            traverseFrames(window, 0, data);

            myArgs.unshift(data);
            myArgs.unshift('ApplicationChanged');
            hub.publish.apply(hub, myArgs);
        },

        onPageLoaded = function () {
            var args = arguments;

            if (!traverseTimeout) {
                traverseTimeout = setTimeout(function () {
                    onTraverseTimeout(args);
                }, 300);
            }
        },

        Mole = function () {
            hub.subscribe('PageLoaded', onPageLoaded);
        };


    return Mole;
});