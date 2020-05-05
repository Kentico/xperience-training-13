/*
    Compatibility package for KenticoCMS
*/
(function ($cmsj) {
    var initBrowser = function () {
        if (!$cmsj.browser) {
            var ua = navigator.userAgent.toLowerCase();
            var match = /(chrome)[ \/]([\w.]+)/.exec(ua) ||
            /(webkit)[ \/]([\w.]+)/.exec(ua) ||
            /(opera)(?:.*version|)[ \/]([\w.]+)/.exec(ua) ||
            /(msie) ([\w.]+)/.exec(ua) ||
            ua.indexOf("compatible") < 0 && /(mozilla)(?:.*? rv:([\w.]+)|)/.exec(ua) || [];

            var matched = {
                browser: match[1] || "",
                version: match[2] || "0"
            };

            var browser = {};

            if (matched.browser) {
                browser[matched.browser] = true;
                browser.version = matched.version;
            }

            // Chrome is Webkit, but Webkit is also Safari.
            if (browser.chrome) {
                browser.webkit = true;
            } else if (browser.webkit) {
                browser.safari = true;
            }

            $cmsj.browser = browser;
        }
    };


    // Init browser property in $cmsj due to backward compatibility
    initBrowser();

})($cmsj)

