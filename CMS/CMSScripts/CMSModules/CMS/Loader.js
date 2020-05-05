cmsdefine(['CMS/EventHub', 'jQuery'], function (hub, $) {
    var overlay,
        loader,
        data,
        bound,
        timeout,
        frame,

        center = function (elem) {
            elem.css("position", "fixed");
            elem.css("top", ($(window).height() / 2) - (elem.outerHeight() / 2));
            elem.css("left", ($(window).width() / 2) - (elem.outerWidth() / 2));
            return elem;
        },
        show = function (overFrame) {
            frame = overFrame;

            if (frame && !bound) {
                bound = true;

                frame.load(hide);
            }

            if (!window.noProgress) {
                delayedShow(1000);
            }
        },
        clear = function () {
            if (timeout) {
                clearTimeout(timeout);
                timeout = null;
            }
        },
        delay = function () {
            if (timeout) {
                delayedShow(1000);
            }
        },
        submitForm = function (delayTimeout, async) {
            if (!window.noProgress) {
                var isAsync = false;
                try {
                    isAsync = Sys.WebForms.PageRequestManager.getInstance()._postBackSettings.async;
                }
                catch (e) {
                }
                if (!isAsync || async) {
                    delayedShow(delayTimeout);
                }
            }
        },
        delayedShow = function (delayTimeout) {
            if (!window.noProgress) {
                clear();
                timeout = setTimeout(showDelayed, delayTimeout);
            }
        },
        showDelayed = function () {
            clear();

            if (overlay.length === 0) {
                // Enusre overlay if it doesn't exist
                overlay = $(data.overlayHtml);
                $(document.body).append(overlay);
            }
            if (loader.length === 0) {
                // Enusre loader if it doesn't exist
                loader = $(data.loaderHtml);
                $(document.body).append(loader);
            }
            var bootstrapClass = "cms-bootstrap";
            if (!$(document.body).hasClass(bootstrapClass)) {
                var bootstrapWrapper = '<div class="' + bootstrapClass + '"></div>';
                loader.wrap(bootstrapWrapper);
                overlay.wrap(bootstrapWrapper);
            }

            overlay.click(hide);

            hub.publish('HideLoader', { except: this });

            center(loader);

            if (frame) {
                var o = frame.offset();
                overlay.css({
                    left: o.left,
                    top: o.top,
                    width: o.width,
                    height: o.height,
                    opacity: '',
                    display: ''
                });
            } else {
                overlay.css({
                    left: 0,
                    top: 0,
                    width: '100%',
                    height: '100%',
                    opacity: '',
                    display: ''
                });
            }

            loader.show();
            overlay.show();
        },
        hideLoader = function (p) {
            if (!p || (p.except !== this)) {
                hide();
            }
        },
        hide = function () {
            clear();

            loader.hide();
            overlay.hide();
        },
        pageUnload = function () {
            show();
        },
        Loader = function (loaderData) {
            data = loaderData;

            // Utilize existing overlay and loader
            overlay = $('#cms-overlayer') || overlay;
            loader = $('#cms-loader') || loader;

            this.delayedShow = delayedShow;
            this.submitForm = submitForm;
            this.show = show;
            this.hide = hide;
            this.delay = delay;

            $(window).one('unload', pageUnload);

            window.Loader = this;

            hub.subscribe('HideLoader', hideLoader);
        };

    return Loader;
});