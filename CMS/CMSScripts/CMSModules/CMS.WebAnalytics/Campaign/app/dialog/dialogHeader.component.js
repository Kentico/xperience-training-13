(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/dialogHeader.component', [])
        .component('cmsDialogHeader', dialogHeader());

    function dialogHeader() {
        return {
            bindings: {
                onClose: '<',
                header: '@'
            },
            templateUrl: 'cms.webanalytics/campaign/dialog/dialogHeader.component.html',
            replace: true,
            controller: controller
        }
    }

    /*@ngInject*/
    function controller($window) {
        var ctrl = this,
            context = $window.top.document,
            $dialogHeader = $cmsj("#dialog-header", context),
            $dialogContent = $dialogHeader.closest('.ui-dialog-content', context);

        initDialog();

        ctrl.maximize = function () {
            ctrl.originalHeight = $dialogContent.height();
            ctrl.originalWidth = $dialogContent.width();
            ctrl.originalOffset = $dialogContent.offset();

            maximizeDialog();
            centerDialog();
            ctrl.maximized = true;
        };

        ctrl.restore = function () {
            restoreDialog();
            ctrl.maximized = false;
        };

        function maximizeDialog () {
            var windowWidth = $window.top.innerWidth,
                windowHeight = $window.top.innerHeight,
                verticalPadding = 24,
                horizontalPadding = 48;

            $dialogContent.width(windowWidth - 2 * horizontalPadding);
            $dialogContent.height(windowHeight - 2 * verticalPadding);
        }

        function restoreDialog() {
            $dialogContent.width(ctrl.originalWidth);
            $dialogContent.height(ctrl.originalHeight);
            $dialogContent.offset(ctrl.originalOffset);
        }

        function centerDialog() {
            $dialogContent.offset({ top: 24, left: 48});
        }

        function initDialog () {
            $dialogHeader.dblclick(function () {
                if (ctrl.maximized) {
                    ctrl.restore();
                }
                else {
                    ctrl.maximize();
                }
            })
        }
    }
}(angular));