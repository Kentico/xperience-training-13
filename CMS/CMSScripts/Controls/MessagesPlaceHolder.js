// MessagesPlaceHolder.js
var cmsLbls = cmsLbls || [];

function CMSHandleLabel(id, description, delay, close, options) {
    options = CMSMessageLoadOptions(options);
    var elm = $cmsj('#' + id);
    window.cmsLbls[id] = elm;
    var lblOp = (options.lblsOpacity / 100.0);
    elm.css('opacity', (delay == 0) ? lblOp : '1');

    CMSRfrLblPos(elm, options);
    $cmsj(window).resize(function () {
        CMSRfrLblPos(elm, options);
        if (options.useRelPlc) {
            CMSRfrRelPlcSize(elm);
        }
    });

    // Prepares empty div to adjust his height and move with unigrid up and down
    var plc = null;
    if (options.useRelPlc && delay <= 0) {
        plc = $cmsj('<div></div>').addClass('lblPlc').addClass(elm.attr('id'));
        elm.after(plc);
    }

    if (description !== '') {
        var desc = $cmsj('<div class="alert-description">' + description + '</div>').hide();
        var lnk = $cmsj('<a>' + options.lblDetails + '</a>').addClass('alert-link');
        elm.find("div").append($cmsj('<span>&nbsp;(</span>')).append(lnk).append($cmsj('<span>)</span>')).append(desc);
        lnk.click(function () {
            desc.slideToggle(0, function () {
                lnk.text(desc.is(':visible') ? options.lblLess : options.lblDetails);

                // After clicking on "see more details" set plc(empty div) a new height to move with unigrid, elm(message)
                CMSRfrRelPlcSize(elm, plc);
            });
        });
    }
    if (delay > 0) {
        elm.mouseenter(function () { elm.stop(true, true).fadeIn('fast'); });
        elm.mouseleave(function () { elm.delay(delay).fadeOut('fast'); });
        elm.delay(delay).fadeOut('fast');
        if (description === '') {
            elm.click(function () { elm.hide(); });
        }
    }
    else {
        if (options.useRelPlc) {
            CMSRfrRelPlcSize(elm, plc);
        }

        if (close) {
            var closeElem = $cmsj('<span></span>').addClass('alert-close');
            closeElem.append($cmsj('<i></i>').addClass('close icon-modal-close').click(function () {
                elm.hide();

                if (plc !== null) {
                    plc.hide();
                }

                if (Object.keys(cmsLbls).length > 1) {
                    CMSRfrLblsPos();
                }
            }));
            closeElem.append($cmsj('<span">Close</span>').addClass('sr-only'));

            elm.append(closeElem);
        }
        elm.hover(function () { elm.addClass("hover"); }, function () { elm.removeClass("hover"); });
    }
}

function CMSMessageLoadOptions(options) {
    options = options || {};
    if (typeof (options.wrpCtrlid) === "undefined") { options.wrpCtrlid = wrpCtrlid; }
    if (typeof (options.lblDetails) === "undefined") { options.lblDetails = lblDetails; }
    if (typeof (options.lblLess) === "undefined") { options.lblLess = lblLess; }
    if (typeof (options.posOffsetY) === "undefined") { options.posOffsetY = posOffsetY; }
    if (typeof (options.posOffsetX) === "undefined") { options.posOffsetX = posOffsetX; }
    if (typeof (options.lblsOpacity) === "undefined") { options.lblsOpacity = lblsOpacity; }
    if (typeof (options.useRelPlc) === "undefined") { options.useRelPlc = useRelPlc; }

    return options;
}

function CMSRfrRelPlcSize(elm, plc) {
    plc = plc || $cmsj('.' + elm.attr('id'));
    if (plc) {
        // Adjust message's width without margin to the placeholder's width.
        elm.css('max-width', plc.outerWidth());

        // Adjust placeholder's height to message's width with margin.
        plc.css('height', elm.outerHeight(true));
    }
}

function CMSRfrLblPos(elm, options) {
    if (elm.is(':hidden') || elm.hasClass("hover")) { return; }
    var opacity = elm.css('opacity');
    elm.css('opacity', 0).css('position', 'static');
    var offset = elm.offset();
    if (elm.hasClass('alert-success')) {
        elm.css('position', 'fixed');
    } else {
        elm.css('position', 'absolute');
    }
    var top = CMSGetPlcPos();

    //Wrapper object
    var ctlrwrpobj = null;
    if (options.wrpCtrlid !== '') {
        ctlrwrpobj = $cmsj('#' + options.wrpCtrlid);
    }

    // Offset top
    var otop = offset.top;
    if (ctlrwrpobj !== null) {
        otop = ctlrwrpobj.offset().top;
    }

    // Ensure wrapper max width
    if (ctlrwrpobj !== null) {
        elm.css('max-width', ctlrwrpobj.width() - 100);
    }

    top = (top > otop) ? top : otop;

    // Add previous sibling's height if exists
    elm.prev(".alert").each(function () {
        var sib = $cmsj(this);
        if (sib.offset().top >= top) {
            top += sib.outerHeight(true);
        }
    });

    elm.css('opacity', opacity);

    var offsetY = options.posOffsetY;
    var offsetX = options.posOffsetX;

    // Add offset to the absolute label possition (if defined)
    if (offsetY > 0) {
        elm.css('top', top + offsetY);
    }

    if (offsetX > 0) {
        elm.css('left', offsetX);
    }
}

function CMSGetPlcPos() {
    var pos = 0;
    $cmsj('.CMSFixPanel, #CMSHeaderPad, .PreviewMenu, .SplitToolbar.Vertical').each(function () { pos += $cmsj(this).height(); });
    return pos;
}

function CMSRfrLblsPos() {
    if (window.cmsLbls !== null) {
        var options = CMSMessageLoadOptions(options);

        for (var key in window.cmsLbls) {
            CMSRfrLblPos(window.cmsLbls[key], options);
        }
    }
}

function CMSHndlLblAnchor(ctrlId, name, options) {
    options = CMSMessageLoadOptions(options);
    var ctrl = $cmsj('#' + ctrlId);
    var parent = ctrl.parent();
    var top = CMSGetPlcPos();
    if (options.useRelPlc) {
        $cmsj('.lblPlc').each(function () { top += $cmsj(this).outerHeight(true); });
    }
    else {
        $cmsj('.alert').each(function () { top += $cmsj(this).outerHeight(true); });
    }
    $cmsj(window).scrollTop(parent.offset().top - top);
    $cmsj('.AnchorFocus').each(function () { $cmsj(this).removeClass('AnchorFocus'); });
    parent.addClass('AnchorFocus');
    var inEl = null;
    var forId = ctrl.prop('for');
    if (typeof (forId) !== "undefined") {
        inEl = $cmsj('#' + forId);
    }
    else {
        inEl = $cmsj('input[name*="$' + name + '$"],textarea[name*="$' + name + '$"]');
    }

    if ((inEl !== null) && (inEl.length > 0)) {
        if (typeof (inEl.focus) === 'function') {
            inEl.focus();
        };

        var ckEditor = window.CKEDITOR;
        if ((typeof (ckEditor) !== 'undefined') && (ckEditor.instances !== null)) {
            var oEditor = ckEditor.instances[inEl.attr("id")];
            if (oEditor !== null) {
                oEditor.focus();
            }
        }
    }
}
