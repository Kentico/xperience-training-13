// This module returns no conflicted version of jQuery.
// The purpose is to permit requiring newest version of jQuery, when
// there are older versions used by legacy code at the same time.
define(['jquery'], function (jq) {
    return jq.noConflict(true);
});