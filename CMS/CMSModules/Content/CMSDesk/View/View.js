var IsCMSDesk = true;

// Refresh tree
function RefreshTree(expandNodeId, selectNodeId) {
    if (parent.RefreshTree) {
        parent.RefreshTree(expandNodeId, selectNodeId);
    }
}

// Refresh selected device
function ChangeDevice(device) {
    // Cancel the current device rotation state -> will use default device rotation
    $cmsj.cookie(CMSView.RotateCookieName, null, { path: '/' });

    if ((parent !== this) && parent.ChangeDevice) {
        parent.ChangeDevice(device);
    }
}

function CloseSplitMode() {
    if (parent != this) {
        parent.CloseSplitMode();
    }
}

function PerformSplitViewRedirect(originalUrl, newCulture, successCallback, errorCallback, mode) {
    if (parent != this) {
        parent.PerformSplitViewRedirect(originalUrl, newCulture, successCallback, errorCallback, mode);
    }
}

var CMSView = {
    PreviewWidth: 0,
    PreviewHeight: 0,
    Rotated: false,
    RotateCookieName: null,
    BodyClassInitialized: false,
    FrameScrollPane: null,
    ScrollSize: 16,
    ViewElem: [],
    DeviceElem: [],
    ResizeContentArea: function () {
        if (!CMSView.ViewElem.length) {
            CMSView.ViewElem = $cmsj('#pageview');
        }
        if (!CMSView.DeviceElem.length) {
            CMSView.DeviceElem = $cmsj('.DeviceFrame');
        }

        var newHeight = document.body.offsetHeight - $cmsj('.preview-edit-panel:first').height();

        if (CMSView.ViewElem.length) {
            if (!CMSView.ViewElem.parent().hasClass('jspPane')) {
                CMSView.ViewElem.height(newHeight);
            }
            else {
                CMSView.DeviceElem.height(newHeight);
            }
        }
    },
    InitializeFrame: function (width, height, rotate) {
        var deviceFrame = $cmsj('.DeviceFrame');

        var topLine = deviceFrame.find('.TopLine');
        var centerLine = deviceFrame.find('.CenterLine');
        var bottomLine = deviceFrame.find('.BottomLine');

        var topLeft = topLine.find('.LeftPiece');
        var topCenter = topLine.find('.CenterPiece');
        var topRight = topLine.find('.RightPiece');

        var centerLeft = centerLine.find('.LeftPiece');
        var centerCenter = centerLine.find('.CenterPiece');
        var centerRight = centerLine.find('.RightPiece');

        var bottomLeft = bottomLine.find('.LeftPiece');
        var bottomCenter = bottomLine.find('.CenterPiece');
        var bottomRight = bottomLine.find('.RightPiece');

        var iFrame = centerCenter.find('iframe');

        if ((width > 0) && (height > 0)) {

            CMSView.PreviewWidth = width;
            CMSView.PreviewHeight = height;

            if (rotate) {
                this.Rotated = true;

                width = CMSView.PreviewHeight;
                height = CMSView.PreviewWidth;

                deviceFrame.addClass('Rotated');
            }
            else {
                this.Rotated = false;

                deviceFrame.removeClass('Rotated');
            }

            if (!this.BodyClassInitialized) {
                this.BodyClassInitialized = true;
                $cmsj('body').addClass('DevicePreview');

                centerCenter.css({ 'visible': 'hidden' });
            }

            topCenter.width(width);
            bottomCenter.width(width);

            centerLeft.height(height);
            centerRight.height(height);

            centerCenter.width(width).height(height);

            iFrame.width(width).height(height).attr('scrolling', 'no').bind('load', function () {
                CMSView.FitIframe();
                CMSView.DeviceWindowResize();
            });

            centerCenter.find('.CenterPanel').width(width).height(height).css({ 'overflow': 'hidden' });

            topLine.width(topLeft.outerWidth() + topCenter.outerWidth() + topRight.outerWidth());

            centerLine.width(centerLeft.outerWidth() + centerCenter.outerWidth() + centerRight.outerWidth());

            bottomLine.width(bottomLeft.outerWidth() + bottomCenter.outerWidth() + bottomRight.outerWidth());

            this.ShowScrollbars([topLeft, topCenter, topRight, topCenter, centerLeft, centerCenter, centerRight, bottomLeft, bottomCenter, bottomRight]);
            this.ResizeContentArea();
            this.FitIframe();

            $cmsj(window).bind('resize', function () {
                CMSView.DeviceWindowResize();
            });
        }
        else {
            deviceFrame.css({ 'margin': '0', 'width': '100%', 'height': '100%' });

            topLeft.width(0).height(0);
            topCenter.width(0).height(0);
            topRight.width(0).height(0);

            centerLeft.width(0).height(0);
            centerCenter.width('100%').height('100%');
            centerRight.width(0).height(0);

            bottomLeft.width(0).height(0);
            bottomCenter.width(0).height(0);
            bottomRight.width(0).height(0);
        }
    },
    FitIframe: function () {

        var devFrame = $cmsj('.DeviceFrame'), iFrame = devFrame.find('iframe'), centerPanel = devFrame.find('.CenterPanel');
        var h, w;
        try {
            var obj = iFrame[0].contentWindow.document.body;

            // Check if embed element is used. This elements doesn't show their dimensions properly
            var isEmbedUsed = (obj && (obj.childElementCount > 0)) ? obj.childNodes[0].nodeName.toLowerCase() == "embed" : false;

            // So use default device dimensions if embed element is used, otherwise use current dimensions
            w = (obj && !isEmbedUsed) ? obj.scrollWidth : centerPanel[0].clientWidth;
            h = (obj && !isEmbedUsed) ? obj.scrollHeight : centerPanel[0].clientHeight;
        } catch (e) {
            // Force dimensions in case unexpected error
            w = centerPanel[0].clientWidth;
            h = centerPanel[0].clientHeight;
        }

        if ((w > 0) && (h > 0)) {

            if (h > CMSView.GetHeight()) {
                h += CMSView.ScrollSize;
            }

            if (w > CMSView.GetWidth()) {
                w += CMSView.ScrollSize;
            }

            iFrame.width(w).height(h);

            if (CMSView.FrameScrollPane) {
                CMSView.FrameScrollPane.reinitialise();
            }

            centerPanel.css({ 'overflow': 'auto' }).jScrollPane({ showArrows: true, includeScrollToPaneSize: false });
            CMSView.FrameScrollPane = centerPanel.data('jsp');
            devFrame.find('.CenterPiece .jspVerticalBar').hide();
            devFrame.find('.CenterPiece .jspHorizontalBar').hide();

            if (CMSView.FrameScrollPane) {
                try {
                    iFrame.contents().find('body').unbind('mouseup').bind('mouseup', function () {
                        var frameScrollPane = null;
                        if (window.CMSView) {
                            frameScrollPane = window.CMSView.FrameScrollPane;
                        }
                        if (window.parent.CMSView) {
                            frameScrollPane = window.parent.CMSView.FrameScrollPane;
                        }
                        if (frameScrollPane) {
                            frameScrollPane.cancelDrag();
                        }
                    });
                }
                catch (error) {
                    // the frame might contain content from another domain so scroll events are not available
                }
            }

            devFrame.find('.CenterPiece').css({ 'visible': 'visible' });
        }
    },
    ShowScrollbars: function (elem) {
        if (elem) {
            for (var i = 0; i < elem.length; i++) {
                elem[i].mouseenter(function () {
                    $cmsj('.DeviceFrame').find('.CenterPiece .jspVerticalBar').show();
                    $cmsj('.DeviceFrame').find('.CenterPiece .jspHorizontalBar').show();
                }).mouseleave(function () {
                    $cmsj('.DeviceFrame').find('.CenterPiece .jspVerticalBar').hide();
                    $cmsj('.DeviceFrame').find('.CenterPiece .jspHorizontalBar').hide();
                });
            }
        }
    },
    DeviceWindowResize: function () {
        var jWin = $cmsj(window),
        devFrame = $cmsj('.DeviceFrame'),
        winWidth = jWin.innerWidth(),
        devWidth = $cmsj('.TopLine').outerWidth();

        if (winWidth < devWidth) {
            var devLeft = devFrame.find('.CenterLine .LeftPiece').outerWidth(),
            devRight = devFrame.find('.CenterLine .RightPiece').outerWidth();

            var leftDelta = ((devWidth - winWidth) / 2) + ((devLeft - devRight) / 2);

            devFrame.scrollLeft(leftDelta);
        }
    },
    GetWidth: function () {
        if (CMSView.Rotated) {
            return CMSView.PreviewHeight;
        }
        else {
            return CMSView.PreviewWidth;
        }
    },
    GetHeight: function () {
        if (CMSView.Rotated) {
            return CMSView.PreviewWidth;
        }
        else {
            return CMSView.PreviewHeight;
        }
    }
};

// Bind resize action to browser resize action 
$cmsj(window).bind('resize', function () {
    if (CMSView.ResizeContentArea) {
        CMSView.ResizeContentArea();
    }
});

// Resize content when DOM is fully loaded  
$cmsj(document).ready(function () {
    if (CMSView.ResizeContentArea) {
        CMSView.ResizeContentArea();
    }
});