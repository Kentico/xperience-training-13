/** 
 * Pin module for pinning objects or applications to dashboard.
 */
cmsdefine(['CMS/EventHub', 'CMS/Application', 'jQuery'], function (hub, application, $) {

    var BreadcrumbPin = function (data) {
        var that = this;

        this._$pinParentElement = $('#' + data.pinId + '-list');
        this._$pinElement = $('#' + data.pinId + '-i');

        hub.subscribe('ApplicationChanged', function (cmsApplications) {
            that._$pinParentElement.hide();

            var flattenedApplications = _.flatten(cmsApplications),
                rootBreadcrumb = flattenedApplications.shift(),
                apps = flattenedApplications.filter(function (app) {
                    return app.breadcrumbs || app.breadcrumbsPin;
                });

            // Get first breadcrumb (the root one) if there is no other breadcrumb object available 
            // since all application with root breadcrumbs can be displayed on dashboard
            var breadcrumb = _.isEmpty(apps) ? rootBreadcrumb : _.last(apps);
            // Show only if it has breadcrumbsPin object also show if its application (level 3) or otherwise it must contain elementGuid.
            if (breadcrumb && breadcrumb.breadcrumbsPin && (!breadcrumb.breadcrumbs || breadcrumb.breadcrumbsPin.elementGuid)) {
                that.show(breadcrumb.breadcrumbsPin);
            }
        });

        hub.subscribe("OverwriteBreadcrumbs", function (breadcrumbs) {
            if (breadcrumbs.pin) {
                that.show(breadcrumbs.pin);
            }
        });
    };


    BreadcrumbPin.prototype.show = function (pinData) {
        var that = this,
            showAsPinned = function () {
                that._$pinElement.removeClass('icon-pin-o');
                that._$pinElement.removeClass('icon-disabled');
                that._$pinElement.addClass('icon-pin');
                that._$pinElement.attr('title', that._$pinElement.attr('data-action-text-unpin'));
            },
            showAsUnpinned = function () {
                that._$pinElement.removeClass('icon-pin');
                that._$pinElement.addClass('icon-pin-o');
                that._$pinElement.addClass('icon-disabled');
                that._$pinElement.attr('title', that._$pinElement.attr('data-action-text-pin'));
            },
            showPin = function (show) {
                if (show) {
                    showAsPinned();
                } else {
                    showAsUnpinned();
                }
            };

        showPin(pinData.isPinned);
        that._$pinParentElement.show();

        // Pin needn't to be rendered every time, therefore bound handler have to be removed
        that._$pinElement.off();
        that._$pinElement.on('click', function () {
            var $pin = $(this);

            if ($pin.attr("data-pending")) {
                return;
            }

            $pin.attr("data-pending", true);
            showPin(!pinData.isPinned);

            $.post(application.getData('applicationUrl') + 'cmsapi/PinHandler/' + (pinData.isPinned ? 'Unpin' : 'Pin'), pinData).done(function () {
                $pin.removeAttr("data-pending");
                pinData.isPinned = !pinData.isPinned;
            });
        });
    };
    
    return BreadcrumbPin;
});