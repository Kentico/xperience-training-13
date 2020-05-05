cmsdefine(['angular','CMS/Filters/Resolve','CMS/Filters/NumberShortener','angular-resource','CMS/UrlHelper','jQueryUI','CMS/Application','CMS/SmartTips/Module','CMS.WebAnalytics/Services/AssetResource','CMS.WebAnalytics/Services/NewsletterResource','Underscore','uiBootstrap','CMS/UITabs','CMS.Forms/Directives/CMSSelect','CMS.Forms/Directives/Select2','CMS.Forms/Directives/CMSRadioButton','CMS.Forms/Directives/CMSTextbox','CMS.Forms/Directives/CMSTextarea','CMS.Forms/Directives/CMSModalDialog','CMS.Forms/Directives/CMSCreatePage','CMS.Forms/Directives/CMSSelectItem','CMS/EventHub','CMS.WebAnalytics/ModalDialog'], function(angular,resolve,shortNumber,ngResource,urlHelper,jQueryUi,application,smartTipModule,assetResourceFactory,newsletterResourceFactory,_,uiBootstrap,Tabs,cmsSelectDirective,cmsSelect2Directive,cmsRadioButtonDirective,cmsTextboxDirective,cmsTextareaDirective,cmsModalDirective,cmsCreatePageDialog,cmsSelectItemDialog,EventHub,ModalDialog) { 
var moduleName = 'cms.webanalytics/campaign/';
return function(dataFromServer) { 
if(angular && resolve && dataFromServer && dataFromServer.resources) { 
resolve(angular, dataFromServer.resources); 
} 
(function (angular, moduleName, shortNumber) {
    'use strict';
    angular.module(moduleName, [
            moduleName + 'app.templates',
            'cms.webanalytics/campaign/cms.forms.module',
            'cms.webanalytics/campaign.component',
            'ui.bootstrap'
    ])
        .controller('app', controller)
        .factory('authorizeInterceptor', ['$q', authorizeInterceptor])
        .factory('httpStatusInterceptor', ['$q', 'resolveFilter', httpStatusInterceptor])
        .factory('httpMethodInterceptor', httpMethodInterceptor)
        .config(['$httpProvider', function ($httpProvider) {
            $httpProvider.interceptors.push('authorizeInterceptor');
            $httpProvider.interceptors.push('httpStatusInterceptor');
            $httpProvider.interceptors.push('httpMethodInterceptor');
        }]);

    shortNumber(angular);

    function controller() {
    }

    // CMSApi does not support PUT and DELETE requests due to security reasons
    // Transform those request to the GET and POST one with appropriate URLs
    function httpMethodInterceptor() {
        return {
            request: function (config) {
                if (config.url.indexOf('cmsapi/newsletters/') >= 0) {
                    return config;
                }

                if (config.method === 'POST') {
                    config.url += '/Post';
                }

                if (config.method === 'PUT') {
                    config.method = 'POST';
                    config.url += '/Put';
                }

                if (config.method === 'DELETE') {
                    config.method = 'POST';
                    config.url += '/Delete';
                }

                return config;
            }
        };
    }

    function authorizeInterceptor($q) {
        return {
            'responseError': function (rejection) {
                // User was signed off, need to redirect to the login page
                if (rejection.status === 403) {
                    var logonPageUrl = rejection.headers('logonpageurl');

                    if (logonPageUrl) {
                        window.top.location.href = logonPageUrl;
                    }
                }

                return $q.reject(rejection);
            }
        };
    }

    function httpStatusInterceptor($q, resolveFilter) {
        var CLIENT_ERROR_CATEGORY = 4,
            SERVER_ERROR_CATEGORY = 5;
        return {
            'responseError': function (rejection) {
                if (isStatusInCategory(rejection.status, CLIENT_ERROR_CATEGORY) && !isSpecialClientErrorStatus(rejection.status)) {
                    rejection.data = resolveFilter('campaign.autosave.failed');
                }

                if (isStatusInCategory(rejection.status, SERVER_ERROR_CATEGORY)) {
                    rejection.data = resolveFilter('campaign.autosave.server.error');
                }

                return $q.reject(rejection);
            }
        };
    }

    function isSpecialClientErrorStatus(status) {
        return status === 403 || status === 400;
    }

    function isStatusInCategory(status, category) {
        return (status / 100|0) === category;
    }

})(angular, moduleName, shortNumber);
(function (angular, dataFromServer, smartTipModule) {
    'use strict';

    controller.$inject = ["campaignService", "assetsService", "tabsService", "resolveFilter"];
    angular.module('cms.webanalytics/campaign.component', [
            'cms.webanalytics/campaign.service',
            'cms.webanalytics/campaign/assets.service',
            'cms.webanalytics/tabs.service',
            'cms.webanalytics/campaign/messages.component',
            'cms.webanalytics/campaign/description.component',
            'cms.webanalytics/campaign/inventory.component',
            'cms.webanalytics/campaign/promotion.component',
            'cms.webanalytics/campaign/reportSetup.component',
            'cms.webanalytics/campaign/statusMessage.component',
            'CMS/Filters.Resolve',
            smartTipModule(angular, dataFromServer.resources)
    ])
        .component('cmsCampaign', campaign())
        .value('activityTypes', dataFromServer.activityTypes);

    function campaign() {
        return {
            templateUrl: 'cms.webanalytics/campaign/campaign.component.html',
            controller: controller
        };
    }

    /*@ngInject*/
    function controller(campaignService, assetsService, tabsService, resolveFilter) {
        var ctrl = this,
            scheduleTabLinkText = resolveFilter('campaign.schedule.link');

        ctrl.inventoryItems = assetsService.getInventoryItems();
        ctrl.promotionItems = assetsService.getPromotionItems();

        ctrl.campaign = campaignService.getCampaign();
        ctrl.siteIsContentOnly = dataFromServer.siteIsContentOnly;
        ctrl.conversions = dataFromServer.conversions || [];
        ctrl.urlAssetTargetRegex = dataFromServer.urlAssetTargetRegex;

        /* Setup smart tips */
        ctrl.promotionSmartTip = dataFromServer.smartTips['promotionSmartTip'];
        ctrl.promotionSmartTip.selector = '#promotionSmartTip';

        ctrl.launchSmartTip = dataFromServer.smartTips['launchSmartTip'];
        ctrl.launchSmartTip.selector = '#launchSmartTip';

        /*
        Add Schedule tab link to the launch section smart tip.
        The link itself will be created in tabs service because the url may change
        (when campaign is saved for the first time (added 'campaignid' parameter))
         */
        ctrl.initLaunchLink = function () {
            // Create clickable link
            var link = $cmsj('<a>' + scheduleTabLinkText + '</a>').click( function() {
                tabsService.navigateScheduleTab();
            });

            $cmsj(ctrl.launchSmartTip.selector + ' .js-st-content').append(link);
        };

    }
}(angular, dataFromServer, smartTipModule));
(function (angular, application, dataFromServer) {
    'use strict';
    
    campaignService.$inject = ["$q", "campaignFactory", "autosaveService", "tabsService", "messagesService", "breadcrumbsService"];
    campaignFactory.$inject = ["$resource"];
    angular.module('cms.webanalytics/campaign.service', [
        'cms.webanalytics/tabs.service',
        'cms.webanalytics/campaign/breadcrumbs.service',
        'cms.webanalytics/campaign/autosave.service',
        'cms.webanalytics/campaign/messages.service'
    ])
        .factory('campaignFactory', campaignFactory)
        .service('campaignService', campaignService);
    
    function campaignFactory($resource) {
        var baseUrl = application.getData('applicationUrl') + 'cmsapi/Campaign/';

        return $resource(baseUrl, {}, {
            'update': { method: 'PUT' }
        });
    }
    
    /*@ngInject*/
    function campaignService($q, campaignFactory, autosaveService, tabsService, messagesService, breadcrumbsService) {
        var that = this;
        that.campaignFormName = 'campaignForm';

        that.getCampaign = function () {
            if (!that.campaign) {
                that.campaign = dataFromServer.campaign;
                that.campaign.isDraft = function () {
                    return that.campaign.status === 'Draft';
                };

                that.campaign.isLaunched = function () {
                    return that.campaign.status === 'Running';
                };

                that.campaign.isFinished = function () {
                    return that.campaign.status === 'Finished';
                };

                that.campaign.isScheduled = function () {
                    return that.campaign.status === 'Scheduled';
                }
            }
            
            return that.campaign;
        };
        
        that.saveCampaign = function (onError) {
            var processOnError = function (response) {
                saveFailed(response);

                if (onError && response.status === 400) {
                    onError(response.data);
                }
            }

            if (that.campaign.campaignID > 0) {
                that.savePromise = campaignFactory.update(that.campaign, saveFinished, processOnError).$promise;
            }
            else {
                that.savePromise = campaignFactory.save(that.campaign, saveFinished, processOnError).$promise;
            }
        };

        that.ensureCampaignExists = function () {
            if (that.campaign.campaignID) {
                // Campaign already exists -> success
                return $q(function(resolve) {resolve(that.campaign)});
            }
            autosaveService.saveForm(that.campaignFormName);

            // Display error message and return reject promise if autosave failed
            if (!that.savePromise) {
                messagesService.clearFormError(that.campaignFormName);
                messagesService.sendFormError(that.campaignFormName, 'campaign.initialSave.validationAlert', true);

                return $q(function(resolve, reject) {reject()});
            }

            // Return save promise created in autosave
            return that.savePromise;
        };

        function saveFinished (campaign) {
            var firstSave = that.campaign.campaignID === 0;
            if (firstSave) {
                tabsService.updateTabLinks(campaign.CampaignID);
            }

            breadcrumbsService.updateBreadcrumbs(campaign, firstSave);

            that.campaign.campaignID = campaign.CampaignID;
            messagesService.sendSuccess('campaign.autosave.finished', true);
        }

        function saveFailed(response) {
            messagesService.sendFormError(that.campaignFormName, response.data);
        }
    }
}(angular, application, dataFromServer));
(function (angular) {
    'use strict';

    autosave.$inject = ["autosaveService"];
    angular.module('cms.webanalytics/campaign/autosave.directive', [
        'cms.webanalytics/campaign/autosave.service'
    ])
        .directive('cmsAutosave', autosave);

    /*@ngInject*/
    function autosave(autosaveService) {
        return {
            restrict: 'A',
            require: '^form',
            scope: {
                callback: '&cmsAutosave'
            },
            link: function ($scope, $element, $attrs, $formController) {
                autosaveService.autosave($scope, $formController, $scope.callback);
            }
        }
    }
}(angular));
(function (angular) {
    'use strict';

    autosaveService.$inject = ["$timeout", "messagesService"];
    angular.module('cms.webanalytics/campaign/autosave.service', [
        'cms.webanalytics/campaign/messages.service'
    ])
        .service('autosaveService', autosaveService);

    /*@ngInject*/
    function autosaveService($timeout, messagesService) {
        var that = this,
            forms = {};

        that.autosave = function ($scope, $form, saveMethod) {
            // Add form to the scope so it can be watched
            var formName = $form.$name;
            if (formName === '') {
                return;
            }

            $scope[formName] = $form;

            forms[formName] = {
                $form: $form,
                saveMethod: saveMethod
            };

            // Watch for changes
            $scope.$watch(formName + '.$dirty', function () {
                if ($form.$dirty) {
                    processFormChange(forms[formName]);
                }
            });
        };

        that.saveForm = function (formName) {
            var formData = forms[formName];

            // Cancel queried save
            cancelSave(formData);
            performSave(formData);

            // Set form untouched so next changes can be detected
            formData.$form.$setPristine();
        };

        function processFormChange (formData) {
            cancelSave(formData);
            
            formData.savePromise = $timeout(function () {
                performSave(formData);
            }, 1000);

            formData.$form.$setPristine();
        }

        function cancelSave (formData) {
            if (formData.savePromise) {
                $timeout.cancel(formData.savePromise);
            }
        }

        function performSave (formData) {
            var formName = formData.$form.$name;
            formData.savePromise = null;
            
            // Validity could be violated since the last check, make sure it still persists
            if (formData.$form.$valid) {
                messagesService.clearFormError(formName);
                formData.saveMethod();
            }
            else {
                messagesService.sendFormError(formName, 'campaign.autosave.validationFailed', true);
            }
        }
    }
}(angular));
(function (angular) {
    'use strict';

    controller.$inject = ["campaignService"];
    angular.module('cms.webanalytics/campaign/description.component', [
            'cms.webanalytics/campaign.service',
            'cms.webanalytics/campaign/autosave.directive',
            'cms.webanalytics/campaign/description/textbox.component',
            'cms.webanalytics/campaign/description/textarea.component'
        ])
        .component('cmsCampaignDescription', description());

    function description() {
        return {
            bindings: {
                campaign: '='
            },
            templateUrl: 'cms.webanalytics/campaign/description/description.component.html',
            replace: true,
            controller: controller
        };
    }

    /*@ngInject*/
    function controller (campaignService) {
        var ctrl = this;
        ctrl.serverValidationMessage = undefined;

        ctrl.saveCampaign = function () {
            ctrl.serverValidationMessage = '';
            campaignService.saveCampaign(function (message) {
                ctrl.serverValidationMessage = message;
            });
            
        };
    }
}(angular));
(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/dialogFooter.component', [])
        .component('cmsDialogFooter', dialogFooter());

    function dialogFooter() {
        return {
            bindings: {
                confirmLabel: '@',
                dismissLabel: '@',
                onConfirm: '<',
                onDismiss: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/dialog/dialogFooter.component.html',
            replace: true
        }
    }
}(angular));

(function (angular) {
    'use strict';

    controller.$inject = ["$window"];
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
(function (angular) {
    'use strict';
    
    controller.$inject = ["assetsService", "confirmationService"];
    angular.module('cms.webanalytics/campaign/inventory.component', [
            'cms.webanalytics/campaign.service',
            'cms.webanalytics/campaign/assets.service',
            'cms.webanalytics/campaign/messages.service',
            'cms.webanalytics/campaign/inventory/addUrlAsset.component',
            'cms.webanalytics/campaign/inventory/createPage.component',
            'cms.webanalytics/campaign/inventory/selectPage.component',
            'cms.webanalytics/campaign/inventory/item.component',
            'cms.webanalytics/campaign/confirmation.service',
            'CMS/Filters.Resolve',
            'ngResource'
    ])
        .component('cmsCampaignInventory', inventory());

    function inventory() {
        return {
            bindings: {
                assets: '=',
                siteIsContentOnly: '<',
                campaign: '=',
                urlAssetTargetRegex: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/inventory/inventory.component.html',
            controller: controller,
            replace: true
        };
    }

    /*@ngInject*/
    function controller(assetsService, confirmationService) {
        var ctrl = this;

        ctrl.addPageAsset = function (pageId) {
            assetsService.addAsset(pageId, "cms.document").then(addAssetToListing);
        };
        
        ctrl.addUrlAsset = function (asset) {
            assetsService.addUrlAsset(asset).then(addAssetToListing);
        };

        ctrl.removeAsset = function (asset) {
            if (confirmationService.canRemoveAsset()) {
                assetsService.removeAsset(asset.assetID).then(function () {
                    delete ctrl.assets[asset.assetID];
                })
            }
        };

        function addAssetToListing(newAsset) {
            ctrl.assets[newAsset.assetID] = newAsset;
        }
    }
}(angular));
(function (angular) {
    'use strict';
    
    controller.$inject = ["$timeout", "messagesService"];
    angular.module('cms.webanalytics/campaign/messages.component', [
        'cms.webanalytics/campaign/messages.service'
    ])
        .component('cmsCampaignMessages', messages());
    
    function messages() {
        return {
            bindings: {
                id: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/messages/messages.component.html',
            replace: true,
            controller: controller
        }
    }

    /*@ngInject*/
    function controller($timeout, messagesService) {
        var ctrl = this;

        ctrl.temporaryMessage = "";
        ctrl.permanentErrors = {};

        messagesService.addListener(ctrl.id, showError, showSuccess);

        ctrl.getPermanentError = function () {
            var firstErrorKey = Object.keys(ctrl.permanentErrors)[0];
            return ctrl.permanentErrors[firstErrorKey];
        };

        function showError (message, context) {
            if (!message) {
                delete ctrl.permanentErrors[context];
            }
            else if (context) {
                ctrl.permanentErrors[context] = message;
            }
            else {
                showTemporary(message, "alert-error");
            }
        }
        
        function showSuccess (message) {
            showTemporary(message, "alert-info");
        }

        function showTemporary (message, messageClass) {
            if (ctrl.getPermanentError()) {
                return;
            }

            ctrl.temporaryMessage = message;
            ctrl.temporaryMessageClass = messageClass;

            cancelMessageTimeout();
            ctrl.messageTimeout = $timeout(function () {
                ctrl.temporaryMessage = "";
            }, 5000);
        }

        function cancelMessageTimeout() {
            if (ctrl.messageTimeout) {
                $timeout.cancel(ctrl.messageTimeout);
            }
        }
    }
}(angular));
(function (angular) {
    'use strict';

    messagesService.$inject = ["resolveFilter"];
    angular.module('cms.webanalytics/campaign/messages.service', [
        'CMS/Filters.Resolve'
    ])
        .service('messagesService', messagesService);

    /*@ngInject*/
    function messagesService(resolveFilter) {
        var that = this,
            listeners = {};

        that.addListener = function (id, onError, onSuccess) {
            listeners[id] = {
                onError: onError,
                onSuccess: onSuccess
            };
        };

        that.sendError = function (message, resolve, listenerId) {
            message = localize(message, resolve);
            send(message, listenerId, 'onError');
        };

        that.sendSuccess = function (message, resolve, listenerId) {
            message = localize(message, resolve);
            send(message, listenerId, 'onSuccess');
        };

        that.sendFormError = function (form, message, resolve, listenerId) {
            message = localize(message, resolve);
            send(message, listenerId, 'onError', form);
        };

        that.clearFormError = function (form, listenerId) {
            send(null, listenerId, 'onError', form);
        };

        function send(message, listenerId, method, context) {

            if (listenerId) {
                listeners[listenerId][method](message);
            }
            else {
                angular.forEach(listeners, function (listener) {
                    listener[method](message, context);
                });
            }
        }

        function localize(message, resolve) {
            return resolve ? resolveFilter(message) : message;
        }
    }
}(angular));
(function (angular) {
    'use strict';

    controller.$inject = ["assetsService", "confirmationService"];
    angular.module('cms.webanalytics/campaign/promotion.component', [
        'cms.webanalytics/campaign/assets.service',
        'cms.webanalytics/campaign/promotion/createEmail.component',
        'cms.webanalytics/campaign/promotion/selectEmail.component',
        'cms.webanalytics/campaign/promotion/promotionItem.component',
        'cms.webanalytics/campaign/confirmation.service',
        'CMS/Filters.Resolve'
    ])
    .component('cmsCampaignPromotion', promotion());

    function promotion() {
        return {
            bindings: {
                items: '=',
                siteIsContentOnly: '<',
                campaign: '='
            },
            templateUrl: 'cms.webanalytics/campaign/promotion/promotion.component.html',
            replace: true,
            controller: controller
        };
    }

    /*@ngInject*/
    function controller(assetsService, confirmationService) {
        var ctrl = this;

        ctrl.addPromotionItem = function (itemId) {
            assetsService.addAsset(itemId, 'newsletter.issue').then(addItemToListing);
        };

        ctrl.removeItem = function (item) {
            if (confirmationService.canRemoveAsset()) {
                assetsService.removeAsset(item.assetID).then(function () {
                    delete ctrl.items[item.assetID];
                })
            }
        };

        function addItemToListing(newItem) {
            ctrl.items[newItem.assetID] = newItem;
        }
    }
}(angular));
(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/reportSetup.component', [
        'cms.webanalytics/campaign/reportSetup/conversions.component',
        'cms.webanalytics/campaign/reportSetup/objective.component',
        'CMS/Filters.Resolve'
    ])
        .component('cmsCampaignReportSetup', reportSetup());

    function reportSetup() {
        return {
            bindings: {
                conversions: '=',
                siteIsContentOnly: '<',
                campaign: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/reportSetup/reportSetup.component.html',
            replace: true
        };
    }
}(angular));
(function (angular) {
    'use strict';

    controller.$inject = ["campaignService", "resolveFilter"];
    angular.module('cms.webanalytics/campaign/statusMessage.component', [
        'cms.webanalytics/campaign.service',
        'CMS/Filters.Resolve'
    ])
        .component('cmsCampaignStatusMessage', statusMessage());

    function statusMessage() {
        return {
            templateUrl: 'cms.webanalytics/campaign/statusMessage/statusMessage.component.html',
            replace: true,
            controller: controller
        }
    }

    /*@ngInject*/
    function controller(campaignService, resolveFilter) {
        var ctrl = this,
            campaign = campaignService.getCampaign();

        ctrl.closed = campaign.isDraft();

        ctrl.close = function () {
            ctrl.closed = true;
        };

        if (campaign.isFinished() || campaign.isScheduled()) {
            ctrl.alertMessage = {
                messageText: campaign.isFinished() ? resolveFilter('campaign.message.finished') : resolveFilter('campaign.message.scheduled'),
                cssClass : 'alert-info',
                icon : 'icon-i-circle'
            }
        } 
        else if (campaign.isLaunched()) {
            ctrl.alertMessage = {
                messageText: resolveFilter('campaign.message.running'),
                cssClass: 'alert-warning',
                icon: 'icon-exclamation-triangle'
            }
        }
    }
}(angular));
(function (angular, assetResourceFactory, newsletterResourceFactory) {
    'use strict';

    assetsService.$inject = ["$q", "campaignService", "assetResource", "messagesService", "serverDataService"];
    angular.module('cms.webanalytics/campaign/assets.service', [
        'cms.webanalytics/campaign.service'
    ])
        .factory('newsletterResource', newsletterResourceFactory)
        .factory('assetResource', assetResourceFactory)
        .service('assetsService', assetsService);

    /*@ngInject*/
    function assetsService($q, campaignService, assetResource, messagesService, serverDataService) {
        var that = this,
            assets = serverDataService.getAssets();

        that.getInventoryItems = function () {
            var inventoryItems = {};
            angular.forEach(assets, function (item) {
                if (item.type !== 'newsletter.issue') {
                    inventoryItems[item.assetID] = item;
                }
            });
            return inventoryItems;
        };

        that.getPromotionItems = function () {
            var promotionItems = {};
            angular.forEach(assets, function (item) {
                if (item.type === 'newsletter.issue') {
                    promotionItems[item.assetID] = item;
                }
            });
            return promotionItems;
        };

        that.addAsset = function (id, type) {
            return campaignService.ensureCampaignExists().then(function () {
                var model = {
                    campaignId: campaignService.getCampaign().campaignID,
                    id: id,
                    type: type
                };
                
                return assetResource.create(model).$promise.then(addAssetFinished, requestFailed);
            })
        };

        that.addUrlAsset = function (asset) {
            return campaignService.ensureCampaignExists().then(function() {
                asset.campaignId = campaignService.getCampaign().campaignID;
                return assetResource.create(asset).$promise.then(addAssetFinished, requestFailed);
            });
        };

        that.updateAsset = function (asset) {
            return assetResource.update(asset).$promise.then(requestFinished, requestFailed);
        };

        that.removeAsset = function (id) {
            var model = {
                campaignAssetId: id
            };

            return assetResource.delete(model).$promise.then(requestFinished, requestFailed);
        };


        function addAssetFinished(response) {
            requestFinished(response);

            return {
                assetID: response.AssetID,
                id: response.ID,
                link: response.Link,
                name: response.Name,
                type: response.Type,
                campaignID: response.CampaignID,
                additionalProperties: response.AdditionalProperties
            };
        }

        function requestFailed(response) {
            if (response && response.data) {
                messagesService.sendError(response.data);
            }
            else {
                messagesService.sendError('campaign.assets.failed', true);
            }

            return $q.reject();
        }

        function requestFinished(response) {
            messagesService.sendSuccess('campaign.autosave.finished', true);
            return response;
        }
    }
}(angular, assetResourceFactory, newsletterResourceFactory));
(function (angular, _, EventHub) {
    'use strict';

    breadcrumbs.$inject = ["serverDataService"];
    angular.module('cms.webanalytics/campaign/breadcrumbs.service', [
        'cms.webanalytics/campaign/serverData.service'
    ])
        .service('breadcrumbsService', breadcrumbs);

    /*@ngInject*/
    function breadcrumbs(serverDataService) {
        var that = this,
            breadcrumbs = serverDataService.getDataFromServer().breadcrumbs;

        that.updateBreadcrumbs = function (campaign, isNew) {
            breadcrumbs.data[1].text = _.escape(campaign.DisplayName);

            if (isNew) {
                breadcrumbs.pin.objectName = campaign.CodeName;
                breadcrumbs.pin.objectSiteName = campaign.SiteName;
                breadcrumbs.pin.isPinned = false;
            } else {
                breadcrumbs.pin = null;
            }

            EventHub.publish("OverwriteBreadcrumbs", breadcrumbs);
        }
    }
}(angular, _, EventHub));
(function (angular, application) {
    'use strict';

    angular.module('cms.webanalytics/campaign/cms.application.service', [])
        .service('applicationService', applicationService);

    function applicationService () {
        this.application = application;
    };

}(angular, application));

(function (angular, cmsModalDirective, cmsCreatePageDialog, cmsSelectItemDialog) {
    "use strict";

    angular.module('cms.webanalytics/campaign/cms.dialogs.module', [])
        .directive('cmsModalDialog', cmsModalDirective)
        .directive('cmsCreatePageDialog', cmsCreatePageDialog)
        .directive('cmsSelectItemDialog', cmsSelectItemDialog);

}(angular, cmsModalDirective, cmsCreatePageDialog, cmsSelectItemDialog));
(function (angular, cmsTextboxDirective, cmsRadioButtonDirective, cmsSelectDirective, cmsSelect2Directive, cmsTextareaDirective) {
    'use strict';

    // Wraps all directives from CMS Forms into one module
    angular.module('cms.webanalytics/campaign/cms.forms.module', [])
        .directive('cmsTextbox', cmsTextboxDirective)
        .directive('cmsTextarea', cmsTextareaDirective)
        .directive('cmsRadioButton', cmsRadioButtonDirective)
        .directive('cmsSelect', cmsSelectDirective)        
        .directive('cmsSelect2', cmsSelect2Directive);

}(angular, cmsTextboxDirective, cmsRadioButtonDirective, cmsSelectDirective, cmsSelect2Directive, cmsTextareaDirective));
(function (angular, urlHelper) {
    'use strict';

    angular.module('cms.webanalytics/campaign/cms.urlHelper.service', [])
        .service('urlHelperService', urlHelperService);
    
    function urlHelperService() {
        this.urlHelper = urlHelper;
    }
}(angular, urlHelper));

(function (angular) {
    'use strict';

    confirmationService.$inject = ["campaignService", "resolveFilter"];
    angular.module('cms.webanalytics/campaign/confirmation.service', [
        'cms.webanalytics/campaign.service',
        'CMS/Filters.Resolve'
    ])
        .service('confirmationService', confirmationService);

    /*@ngInject*/
    function confirmationService(campaignService, resolveFilter) {
        var that = this,
            config = getConfig();

        that.canRemoveAsset = can.bind(this, 'remove', 'asset');
        that.canRemoveObjective = can.bind(this, 'remove', 'objective');
        that.canEditObjective = can.bind(this, 'edit', 'objective');

        that.canRemoveConversion = function (isFunnel) {
            return isFunnel ? can('remove', 'funnelStep') : can('remove', 'conversion');
        };

        that.canEditConversion = function (isFunnel) {
            return isFunnel ? can('edit', 'funnelStep') : can('edit', 'conversion');
        };

        function can(action, object) {
            var status = campaignService.getCampaign().status,
                message = config[status] && config[status][action] && config[status][action][object];

            return !message || confirm(resolveFilter(message));
        }

        function getConfig() {
            var config = {
                Draft: {
                    remove: {
                        asset: 'campaign.asset.deleteconfirmation',
                        conversion: 'campaign.conversion.deleteconfirmation',
                        funnelStep: 'campaign.funnelstep.deleteconfirmation',
                        objective: 'campaign.objective.deleteconfirmation'
                    }
                },
                Running: {
                    remove: {
                        asset: 'campaign.asset.deleteconfirmation.running',
                        conversion: 'campaign.conversion.deleteconfirmation.running',
                        funnelStep: 'campaign.funnelstep.deleteconfirmation.running',
                        objective: 'campaign.objective.deleteconfirmation.running'
                    },
                    edit: {
                        conversion: 'campaign.conversion.editconfirmation.running',
                        funnelStep: 'campaign.funnelstep.editconfirmation.running',
                        objective: 'campaign.objective.editconfirmation.running'
                    }
                }
            };

            /* Both Draft and Scheduled campaign have the same confirmation messages */
            config.Scheduled = config.Draft;
            return config;
        }
    }
}(angular));
(function (angular) {
    'use strict';

    conversionResource.$inject = ["$resource", "applicationService"];
    conversionsService.$inject = ["$q", "conversionResource", "campaignService", "messagesService"];
    angular.module('cms.webanalytics/campaign/conversions.service', [
        'cms.webanalytics/campaign/messages.service',
        'cms.webanalytics/campaign/cms.application.service',
        'cms.webanalytics/campaign.service'
    ])
        .factory('conversionResource', conversionResource)
        .service('conversionsService', conversionsService);

    /*@ngInject*/
    function conversionResource($resource, applicationService) {
        var baseUrl = applicationService.application.getData('applicationUrl') + 'cmsapi/CampaignConversion';

        return $resource(baseUrl, {}, {
            'update': { method: 'PUT' },
            'create': { method: 'POST' },
            'delete': { method: 'DELETE' }
        });
    }

    /*@ngInject*/
    function conversionsService($q, conversionResource, campaignService, messagesService) {
        var that = this;

        that.addConversion = function (model) {
            return campaignService.ensureCampaignExists().then(function () {
                model.campaignId = campaignService.getCampaign().campaignID;

                return conversionResource.create(model).$promise.then(requestFinished, requestFailed);
            })
        };

        that.updateConversion = function (model) {
            return conversionResource.update(model).$promise.then(requestFinished, requestFailed);
        };

        that.removeConversion = function (id) {
            var model = {
                conversionID: id
            };

            return conversionResource.delete(model).$promise.then(requestFinished, requestFailed);
        };

        function requestFailed(response) {
            if (response && response.data) {
                messagesService.sendError(response.data);
            }
            else {
                messagesService.sendError('campaign.assets.failed', true);
            }

            return $q.reject();
        }

        function requestFinished(response) {
            messagesService.sendSuccess('campaign.autosave.finished', true);
            return response;
        }
    }
}(angular));
(function (angular) {
    'use strict';

    objectiveResource.$inject = ["$resource", "applicationService"];
    objectiveService.$inject = ["$q", "serverDataService", "objectiveResource", "campaignService", "messagesService"];
    angular.module('cms.webanalytics/campaign/objective.service', [
        'cms.webanalytics/campaign/messages.service',
        'cms.webanalytics/campaign/cms.application.service',
        'cms.webanalytics/campaign.service',
        'cms.webanalytics/campaign/serverData.service'
    ])
        .factory('objectiveResource', objectiveResource)
        .service('objectiveService', objectiveService);

    /*@ngInject*/
    function objectiveResource($resource, applicationService) {
        var baseUrl = applicationService.application.getData('applicationUrl') + 'cmsapi/CampaignObjective';

        return $resource(baseUrl, {}, {
            'update': { method: 'POST' },
            'create': { method: 'POST' },
            'delete': { method: 'DELETE' }
        });
    }

    /*@ngInject*/
    function objectiveService($q, serverDataService, objectiveResource, campaignService, messagesService) {
        var that = this,
            resetObjectiveCallback;

        that.addObjective = function (model) {
            return campaignService.ensureCampaignExists().then(function() {
                model.campaignID = campaignService.getCampaign().campaignID;
                return objectiveResource.create(model).$promise.then(requestFinished, requestFailed);
            });
        };

        that.updateObjective = function (model) {
            return objectiveResource.update(model).$promise.then(requestFinished, requestFailed);
        };

        that.removeObjective = function (id) {
            var model = {
                objectiveID: id
            };
            return objectiveResource.delete(model).$promise.then(requestFinished, requestFailed);
        };

        that.getObjective = function () {
            return serverDataService.getObjective();
        };

        that.registerResetObjectiveCallback = function(callback) {
            resetObjectiveCallback = callback;
        }

        that.resetObjective = function(conversion) {
            if (resetObjectiveCallback && typeof(resetObjectiveCallback) === 'function') {
                resetObjectiveCallback(conversion);
            }
        }

        function requestFailed(response) {
            if (response && response.data) {
                messagesService.sendError(response.data);
            }
            else {
                messagesService.sendError('campaign.assets.failed', true);
            }

            return $q.reject();
        }

        function requestFinished(response) {
            messagesService.sendSuccess('campaign.autosave.finished', true);
            return response;
        }
    }
}(angular));
(function (angular, dataFromServer) {
    'use strict';
    
    angular.module('cms.webanalytics/campaign/serverData.service', [])
        .service('serverDataService', serverDataService);

    function serverDataService () {
        this.getDataFromServer = function () {
            return dataFromServer;
        };

        this.getCreatePageDialogUrl = function () {
            return dataFromServer.createPageDialogUrl;
        };

        this.getEmailRegexp = function () {
            return dataFromServer.emailRegexp;
        };

        this.getSelectEmailDialogUrl = function () {
            return dataFromServer.selectEmailDialogUrl;
        };

        this.getAssets = function () {
            return dataFromServer.assets;
        }

        this.isSiteContentOnly = function () {
            return dataFromServer.siteIsContentOnly;
        }

        this.getUrlAssetTargetRegex = function () {
            return dataFromServer.urlAssetTargetRegex;
        }

        this.getObjective = function () {
            return dataFromServer.objective;
        }
        
    };
}(angular, dataFromServer));

(function (angular, Tabs) {
    'use strict';

    angular.module('cms.webanalytics/tabs.service', [])
        .service('tabsService', tabsService);

    /*@ngInject*/
    function tabsService() {
        var that = this,
            $tabsElement = getTabsElement(),
            $scheduleTab = getScheduleTab();

        that.updateTabLinks = function (campaignId) {
            var tabs = new Tabs();

            tabs.ensureQueryParamForTabs($tabsElement, "campaignid", campaignId);
            tabs.ensureQueryParamForTabs($tabsElement, "objectid", campaignId);
        };

        that.navigateScheduleTab = function () {
            $scheduleTab.click();
        };

        function getTabsElement() {
            return $cmsj("ul.nav.nav-tabs", window.parent.document);
        }

        function getScheduleTab() {
            return $cmsj($tabsElement.find('li[data-href] > a')[1]);
        }
    }
}(angular, Tabs));
(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/description/textbox.component', [])
        .component('cmsCampaignTextbox', textbox());

    function textbox() {
        return {
            bindings: {
                id: '@',
                label: '@',
                pattern: '@',
                required: '@',
                maxlength: '@',
                value: '=',
                patternError: '@',
                enabled: '<',
                formElement: '<',
                serverValidationMessage: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/description/textbox/textbox.component.html',
            replace: true
        };
    }
}(angular));
(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/description/textarea.component', [])
        .component('cmsCampaignTextarea', textarea());

    function textarea() {
        return {
            bindings: {
                value: '=',
                title: '@',
                rows: '@',
                label: '@',
                id: '@'
            },
            templateUrl: 'cms.webanalytics/campaign/description/textarea/textarea.component.html',
            replace: true
        }
    }
}(angular));
(function (angular) {
    'use strict';

    controller.$inject = ["serverDataService"];
    angular.module('cms.webanalytics/campaign/inventory/createPage.component', [
        'cms.webanalytics/campaign/serverData.service',
        'cms.webanalytics/campaign/cms.dialogs.module'
    ])
        .component('cmsCreatePage', createPage());

    function createPage() {
        return {
            bindings: {
                onCreate: '<',
                enabled: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/inventory/createPage/createPage.component.html',
            replace: true,
            controller: controller
        }
    }

    /*@ngInject*/
    function controller (serverDataService) {
        var ctrl = this;
        
        ctrl.dialogUrl = serverDataService.getCreatePageDialogUrl();

        ctrl.dataChange = function () {
            if (ctrl.newPageAsset) {
                ctrl.onCreate(ctrl.newPageAsset);
            }
        };

        ctrl.dataClick = function () {
            return true;
        }
    }
}(angular));
(function (angular) {
    'use strict';

    controller.$inject = ["$uibModal"];
    modalController.$inject = ["$scope", "$uibModalInstance", "urlAssetTargetRegex"];
    angular.module('cms.webanalytics/campaign/inventory/addUrlAsset.component', [
        'cms.webanalytics/campaign/dialogHeader.component',
        'cms.webanalytics/campaign/dialogFooter.component'
    ])
        .component('cmsAddUrlAsset', addUrlAsset());

    function addUrlAsset () {
        return {
            bindings: {
                onAdd: '<',
                campaign: '<',
                urlAssetTargetRegex: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/inventory/addUrlAsset/addUrlAsset.component.html',
            controller: controller,
            replace: true
        }
    }

    /*@ngInject*/
    function controller($uibModal) {
        var ctrl = this;

        ctrl.openDialog = function () {
            $uibModal.open({
                size: { height: '416px', width: '600px' },
                templateUrl: 'cms.webanalytics/campaign/inventory/addUrlAsset/addUrlAsset.dialog.html',
                bindToController: true,
                controllerAs: '$ctrl',
                controller: modalController,
                resolve: {
                    urlAssetTargetRegex: function () { return ctrl.urlAssetTargetRegex; }
                }
            })
                .result.then(onConfirm);
        };

        function onConfirm(item) {
            if (item) {
                item.campaignID = ctrl.campaign.campaignID;
            }

            ctrl.onAdd(item);
        }
    }

    /*@ngInject*/
    function modalController($scope, $uibModalInstance, urlAssetTargetRegex) {
        var ctrl = this;
        ctrl.urlAssetTargetRegex = urlAssetTargetRegex;

        ctrl.confirm = function () {
            if ($scope.newAssetUrlForm.$invalid) {
                $scope.newAssetUrlForm.$setSubmitted();
                return;
            }

            $uibModalInstance.close(prepareAssetToCreate());
        };

        ctrl.dismiss = function () {
            $uibModalInstance.dismiss();
        };

        function prepareAssetToCreate() {
            return {
                type: "analytics.campaignasseturl",
                id: 0,
                name: ctrl.pageTitle,
                link: ctrl.target
            };
        }
    }

}(angular));
(function (angular) {
    'use strict';

    controller.$inject = ["$uibModal"];
    modalController.$inject = ["$uibModalInstance", "$interpolate", "applicationService"];
    angular.module('cms.webanalytics/campaign/inventory/selectPage.component', [
        'cms.webanalytics/campaign/dialogHeader.component',
        'cms.webanalytics/campaign/dialogFooter.component',
        'cms.webanalytics/campaign/cms.application.service',
        'CMS/Filters.Resolve'
    ])
        .component('cmsSelectPage', component());

    function component() {
        return {
            bindings: {
                enabled: '<',
                onSelect: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/inventory/selectPage/selectPage.component.html',
            controller: controller,
            replace: true
        };
    }

    /*@ngInject*/
    function controller($uibModal) {
        var ctrl = this;

        ctrl.openDialog = function () {
            $uibModal.open({
                size: { height: '220px', width: '500px' },
                templateUrl: 'cms.webanalytics/campaign/inventory/selectPage/selectPage.dialog.html',
                bindToController: true,
                controllerAs: '$ctrl',
                controller: modalController
            })
                .result.then(ctrl.onConfirm);
        };

        ctrl.onConfirm = function (item) {
            if (item) {
                ctrl.onSelect(item);
            }
        };
    }

    /*@ngInject*/
    function modalController($uibModalInstance, $interpolate, applicationService) {
        var ctrl = this,
            application = applicationService.application;

        ctrl.restUrl = application.getData('applicationUrl') + 'cmsapi/CampaignConversionPage';
        ctrl.restUrlParams = { objType: "page" };

        ctrl.resultTemplate = function (selectedPage) {
            var itemTemplate =
                    '<div class="campaign-conversion-page-item ">' +
                    '<div class="campaign-conversion-page-icon"> {{icon}} </div>' +
                    '<div class="campaign-conversion-page-content">' +
                    '<strong> {{text}} </strong>' +
                    '<div> {{path}} </div>' +
                    '</div>' +
                    '</div>',

                pageView = angular.copy(selectedPage);

            // Ensure escaping for path and text
            pageView.path = _.escape(pageView.path);
            pageView.text = _.escape(pageView.text);

            return $interpolate(itemTemplate)(pageView);
        };

        ctrl.isSelectionValid = function () {
            return ctrl.detail != null ? ctrl.detail.id > 0 : true;
        }

        ctrl.confirm = function () {
            ctrl.form.$setSubmitted();
            if (ctrl.isSelectionValid()) {
                $uibModalInstance.close(ctrl.detail.id);
            }
        };

        ctrl.dismiss = function () {
            $uibModalInstance.dismiss();
        }
    }
}(angular));

(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/inventory/item.component', [
            'cms.webanalytics/campaign/inventory/item/getLink.component'
        ])
        .component('cmsCampaignInventoryItem', item());

    function item() {
        return {
            bindings: {
                asset: '=',
                siteIsContentOnly: '<',
                editable: '<',
                utmCode: '<',
                removeAsset: '&'
            },
            templateUrl: 'cms.webanalytics/campaign/inventory/item/item.component.html',
            controller: controller
        };
    }

    function controller() {
        var ctrl = this;

        ctrl.isDeleted = function () {
            return ctrl.asset.id === -1;
        };

        ctrl.isExistingDocument = function () {
            return !ctrl.isDeleted() && (ctrl.asset.type === "cms.document");
        };

        ctrl.isPublished = function () {
            return ctrl.asset.additionalProperties.isPublished;
        };
    }
}(angular));
(function (angular){
    'use strict';

    controller.$inject = ["$uibModal"];
    angular.module('cms.webanalytics/campaign/promotion/createEmail.component', [
        'cms.webanalytics/campaign/promotion/createEmail.controller'
    ])
        .component('cmsCreateEmail', createEmail());

    function createEmail() {
        return {
            bindings: {
                onCreate: '<',
                enabled: '<',
                items: '='
            },
            templateUrl: 'cms.webanalytics/campaign/promotion/createEmail/createEmail.component.html',
            replace: true,
            controller: controller
        }
    }

    /*@ngInject*/
    function controller ($uibModal) {
        var ctrl = this;

        ctrl.openDialog = function () {
            $uibModal.open({
                size: {width: "50%", height: "80%"},
                templateUrl: 'cms.webanalytics/campaign/promotion/createEmail/createEmail.dialog.html',
                bindToController: true,
                controllerAs: '$ctrl',
                controller: 'CreateEmailController',
                resolve: {
                    items: function () {
                        return ctrl.items;
                    }
                }
            })
                .result.then(ctrl.onCreate);
        };
    }
}(angular));
(function (angular, _) {

    controller.$inject = ["$scope", "$q", "$uibModalInstance", "items", "newsletterResource", "messagesService", "serverDataService"];
    angular.module('cms.webanalytics/campaign/promotion/createEmail.controller', [])
        .controller('CreateEmailController', controller);

    /*@ngInject*/
    function controller($scope, $q, $uibModalInstance, items, newsletterResource, messagesService, serverDataService) {
        var ctrl = this;
        
        ctrl.items = items;
        ctrl.EMAIL_REGEXP = serverDataService.getEmailRegexp();
        ctrl.emailCampaignType = _.isEmpty(items) ? 'new' : 'assign';
        ctrl.data = {};

        ctrl.emailPattern = { value: ctrl.EMAIL_REGEXP, display: 'afterSubmission' };
        ctrl.emailCampaignTypeOptions = [
        {
            id: 'new-email-new-campaign',
            label: 'campaign.create.email.select.type.new',
            value: 'new'
        },
        {
            id: 'assign-email-campaign',
            label: 'campaign.create.email.select.type.assign',
            value: 'assign'
        }];


        var campaignsPromise = createCampaignsPromise(),
            templatePromise = createTemplatePromise();

        ctrl.isNewCampaignType = function () {
            return ctrl.emailCampaignType === 'new';
        };

        $q.all([campaignsPromise, templatePromise]).catch(function (error) {
            requestFailed(error);
            $uibModalInstance.dismiss();
        });

        ctrl.dismiss = function () {
            $uibModalInstance.dismiss();
        };

        ctrl.confirm = function () {
            if ($scope.newEmailForm.$invalid) {
                $scope.newEmailForm.$setSubmitted();
                return;
            }

            createIssueModelPromise().then(function (issueModel) {
                ctrl.newIssueModel = issueModel;
            }).catch(function (error) {
                requestFailed(error);
            }).finally(function () {
                $uibModalInstance.close(ctrl.newIssueModel.id);
            });
        };

        function createCampaignsPromise() {
            return newsletterResource.getAllEmailCampaigns().$promise.then(function (data) {
                return newslettersToOptions(data);
            }).then(function (options) {
                return newsletterResource.issues({ issueIds: _.pluck(ctrl.items, 'id') })
                    .$promise
                    .then(function (issueModels) {
                        return createHints(options, issueModels);
                    });
            }).then(function (options) {
                // Assign hints to html select directive
                ctrl.data.emailCampaigns = options;
            });
        }

        function createTemplatePromise() {
            return newsletterResource.templates(function (data) {
                var templates = data.map(function (template) {
                    return {
                        id: template.id,
                        name: template.displayName,
                        type: template.type.toLowerCase()
                    }
                });
                ctrl.data.emailTemplates = _.groupBy(templates, 'type');
            }).$promise;
        }

        function requestFailed(response) {
            if (response && response.data) {
                messagesService.sendError(response.data);
            }
        }

        function newslettersToOptions(newsletters) {
            // Convert to select option format
            return _.map(newsletters, function (newsletter) {
                return {
                    id: newsletter.id,
                    name: newsletter.displayName,
                    templates: newsletter.issueTemplates
                }
            });
        }

        function createHints(options, issueModels) {
            if (!issueModels || !issueModels.length) {
                return options;
            }

            var newsletters = _.pluck(issueModels, 'newsletterId'),
                hints = options.filter(function (option) {
                    return newsletters.indexOf(option.id) >= 0;
                });

            if (!hints || !hints.length) {
                return options;
            }

            hints.push({
                id: 0,
                name: '-------',
                disabled: true
            });

            return hints.concat(options);
        }

        function prepareEmailCampaignToCreate() {
            return {
                displayName: ctrl.emailDisplayName,
                senderName: ctrl.emailSenderName,
                senderEmail: ctrl.emailSenderAddress,
                unsubscriptionTemplateId: ctrl.templateUnsubscription,
                issueTemplateId: ctrl.templateIssue
            }
        }

        function createIssueModelPromise() {
            if (ctrl.emailCampaignType === 'new') {
                var emailCapmaign = prepareEmailCampaignToCreate();

                // Create static email campaign and then email
                return newsletterResource
                    .createEmailCampaign(emailCapmaign)
                    .$promise.then(function (newsletterModel) {
                        return createNewIssuePromise(newsletterModel.id, ctrl.emailSubject, ctrl.templateIssue);
                    });
            }
            else {
                // Create email only
                return createNewIssuePromise(ctrl.emailCampaignSelect.id, ctrl.emailSubject, ctrl.templateIssue.id);
            }
        }

        function createNewIssuePromise(id, subject, template) {
            return newsletterResource.createNewIssue({ newsletterId: id, templateId: template}, '"' + subject + '"').$promise;
        }
    }

}(angular, _));
(function (angular) {
    'use strict';

    controller.$inject = ["$scope", "assetsService"];
    angular.module('cms.webanalytics/campaign/promotion/promotionItem.component', [
        'cms.webanalytics/campaign/autosave.directive',
        'cms.webanalytics/campaign/assets.service'
    ])
        .component('cmsCampaignPromotionItem', component());

    function component() {
        return {
            bindings: {
                asset: '=',
                removeItem: '&',
                editable: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/promotion/promotionItem/promotionItem.component.html',
            controller: controller,
            replace: true
        }
    }

    /*@ngInject*/
    function controller($scope, assetsService) {
        var ctrl = this;

        ctrl.inputId = 'email_' + ctrl.asset.id;
        ctrl.formName = ctrl.asset.id === -1 ? '' : 'promotionForm' + ctrl.asset.id;

        ctrl.getUtmSourceField = function () {
            if (!ctrl.utmSourceField && !ctrl.isDeleted()) {
                ctrl.utmSourceField = $scope[ctrl.formName][ctrl.inputId];
            }

            return ctrl.utmSourceField;
        };

        ctrl.isDeleted = function () {
            return ctrl.asset.id === -1;
        };

        ctrl.save = function () {
            assetsService.updateAsset(ctrl.asset);
        };
    }
}(angular));
(function (angular){
    'use strict';

    controller.$inject = ["serverDataService"];
    angular.module('cms.webanalytics/campaign/promotion/selectEmail.component', [
        'cms.webanalytics/campaign/serverData.service',
        'cms.webanalytics/campaign/cms.dialogs.module'
    ])
        .component('cmsSelectEmail', selectItem());

    function selectItem() {
        return {
            bindings: {
                onSelect: '<',
                enabled: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/promotion/selectEmail/selectEmail.component.html',
            replace: true,
            controller: controller
        }
    }

    /*@ngInject*/
    function controller(serverDataService) {
        var ctrl = this;

        ctrl.dialogUrl = serverDataService.getSelectEmailDialogUrl();

        ctrl.onChange = function () {
            if (ctrl.newEmailAsset) {
                ctrl.onSelect(ctrl.newEmailAsset);
            }
        };

        ctrl.onClick = function () {
            return true;
        }
    }
}(angular));
(function (angular, _) {
    'use strict';

    controller.$inject = ["confirmationService", "conversionsConfigurationService", "conversionsService", "objectiveService"];
    angular.module('cms.webanalytics/campaign/reportSetup/conversions.component', [
        'cms.webanalytics/campaign/conversions.service',
        'cms.webanalytics/campaign/reportSetup/conversionsConfiguration.service',
        'cms.webanalytics/campaign/reportSetup/conversions/conversion.component',
        'cms.webanalytics/campaign/reportSetup/conversions/addConversion.component',
        'cms.webanalytics/campaign/confirmation.service',
        'cms.webanalytics/campaign/objective.service',
        'CMS/Filters.Resolve'
    ])
    .component('cmsCampaignConversions', campaignConversions());

    function campaignConversions() {
        return {
            bindings: {
                isFunnel: '<',
                conversions: '=',
                siteIsContentOnly: '<',
                campaign: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/reportSetup/conversions/conversions.component.html',
            controller: controller,
            replace: true
        };
    }

    /*@ngInject*/
    function controller(confirmationService, conversionsConfigurationService, conversionsService, objectiveService) {
        var ctrl = this,
            conversionToRemove;

        ctrl.configuration = conversionsConfigurationService.getConfiguration(ctrl.isFunnel);

        ctrl.addConversionToList = function (conversion) {
            if (!containsConversion(conversion)) {
                ctrl.conversions.push(conversion);
            }
        };

        ctrl.removeConversion = function (conversion) {
            if (confirmationService.canRemoveConversion(ctrl.isFunnel)) {
                conversionToRemove = conversion;
                conversionsService.removeConversion(conversion.id).then(ctrl.removeConversionFinished);
            }
        };

        ctrl.removeConversionFinished = function () {
            objectiveService.resetObjective(conversionToRemove);
            ctrl.conversions.splice(ctrl.conversions.indexOf(conversionToRemove), 1);
        };

        ctrl.isRemovable = function () {
            if (ctrl.isFunnel) {
                return ctrl.campaign.isDraft() || !ctrl.campaign.isFinished();
            }
            else {
                var nonFunnelConversions = ctrl.conversions.filter(function (conversion) {
                    return !conversion.isFunnelStep;
                });

                return ctrl.campaign.isDraft() || (!ctrl.campaign.isFinished() && nonFunnelConversions.length !== 1);
            }
        };

        function containsConversion(conversion) {
            return _.find(ctrl.conversions, function (c) {
                return c.id === conversion.id
            });
        }
    }
}(angular, _));
(function (angular) {
    'use strict';

    configurationService.$inject = ["resolveFilter"];
    angular.module('cms.webanalytics/campaign/reportSetup/conversionsConfiguration.service', [
        'CMS/Filters.Resolve'
    ])
        .service('conversionsConfigurationService', configurationService);

    /*@ngInject*/
    function configurationService(resolveFilter) {
        var configuration = initConfiguration();

        this.getConfiguration = function (isFunnel) {
            return isFunnel ? configuration.funnel : configuration.conversions;
        };

        function initConfiguration() {
            return {
                conversions: {
                    heading: resolveFilter('campaign.conversions'),
                    description: resolveFilter('campaign.conversions.description'),
                    dialogHeading: resolveFilter('campaign.conversions.defineconversion'),
                    addButtonLabel: resolveFilter('campaign.conversions.add'),
                    addButtonTitle: resolveFilter('campaign.conversions.add.title'),
                },

                funnel: {
                    heading: resolveFilter('campaign.journey'),
                    description: resolveFilter('campaign.journey.description'),
                    dialogHeading: resolveFilter('campaign.journey.definestep'),
                    addButtonLabel: resolveFilter('campaign.journey.add'),
                    addButtonTitle: resolveFilter('campaign.journey.add.title'),
                    additionalClass: 'separator',
                }
            }
        }
    }
}(angular));
(function (angular, _) {
    'use strict';

    controller.$inject = ["$scope", "$uibModalInstance", "$interpolate", "conversion", "title", "activityTypes", "resolveFilter", "applicationService", "serverDataService"];
    angular.module('cms.webanalytics/campaign/reportSetup/modifyConversion.controller', [
        'cms.webanalytics/campaign/reportSetup/dialog/selectParameter.directive',
        'cms.webanalytics/campaign/cms.application.service'
    ])
        .controller('ModifyConversionController', controller);

    /*@ngInject*/
    function controller($scope, $uibModalInstance, $interpolate, conversion, title, activityTypes, resolveFilter, applicationService, serverDataService) {
        var ctrl = this,
            application = applicationService.application,
            isSiteContentOnly = serverDataService.isSiteContentOnly(),
            selectedActivityType = 'pagevisit';
        
        ctrl.title = title;
        ctrl.conversion = angular.copy(conversion || { activityType: 'pagevisit' });
        ctrl.activityTypes = prepareActivityTypes(activityTypes, ctrl.conversion.activityType);

        ctrl.urlAssetTargetRegex = serverDataService.getUrlAssetTargetRegex();
        ctrl.selectedConversionItem = {
            id: ctrl.conversion.itemID || 0,
            text: ctrl.conversion.name,
            url: ctrl.conversion.url
        };

        ctrl.showContentOnlyPageVisitConfiguration = function() {
            return isSiteContentOnly && (selectedActivityType === 'pagevisit');
        };

        var resultTemplate = function (selectedPage) {
            var itemTemplate =
                    '<div class="campaign-conversion-page-item ">' +
                        '<div class="campaign-conversion-page-icon"> {{icon}} </div>' +
                        '<div class="campaign-conversion-page-content">' +
                        '<strong> {{text}} </strong>' +
                        '<div> {{path}} </div>' +
                        '</div>' +
                        '</div>',
                         
                pageView = angular.copy(selectedPage);

            // Ensure escaping for path and text
            pageView.path = _.escape(pageView.path);
            pageView.text = _.escape(pageView.text);

            return $interpolate(itemTemplate)(pageView);
        };


        var activityTypesConfiguration = {
            pagevisit: {
                selectorLabel: resolveFilter('campaign.conversion.pageselector'),
                areParametersRequired: !isSiteContentOnly,
                errorMessage: resolveFilter('campaign.conversion.pageisrequired'),
                configuration: {
                    restUrl: application.getData('applicationUrl') + 'cmsapi/CampaignConversionPage',
                    restUrlParams: { objType: 'page' },
                    resultTemplate: resultTemplate,
                    isRequired: true
                }
            },
            purchasedproduct: {
                selectorLabel: resolveFilter('campaign.conversion.productselector'),
                areParametersRequired: true,
                errorMessage: resolveFilter('campaign.conversion.productisrequired'),
                configuration: {
                    restUrl: application.getData('applicationUrl') + 'cmsapi/CampaignConversionPage',
                    restUrlParams: { objType: 'product' },
                    resultTemplate: resultTemplate,
                    isRequired: true
                }
            },
            purchase: {
                selectorLabel: '',
                areParametersRequired: false,
                configuration: {}
            },
            userregistration: {
                selectorLabel: '',
                areParametersRequired: false,
                configuration: {}
            },
            bizformsubmit: {
                selectorLabel: resolveFilter('campaign.conversion.formselector'),
                areParametersRequired: true,
                errorMessage: resolveFilter('campaign.conversion.formisrequired'),
                configuration: {
                    restUrl: application.getData('applicationUrl') + 'cmsapi/CampaignConversionItem',
                    restUrlParams: { objType: 'cms.form' },
                    isRequired: true
                }
            },
            eventbooking: {
                selectorLabel: resolveFilter('campaign.conversion.eventselector'),
                areParametersRequired: true,
                errorMessage: resolveFilter('campaign.conversion.eventrequired'),
                configuration: {
                    restUrl: application.getData('applicationUrl') + 'cmsapi/CampaignConversionPage',
                    restUrlParams: { objType: 'event' },
                    isRequired: true,
                    allowAny: true
                }
            },
            internalsearch: {
                selectorLabel: '',
                areParametersRequired: false,
                configuration: {}
            },
            productaddedtoshoppingcart: {
                selectorLabel: resolveFilter('campaign.conversion.productselector'),
                areParametersRequired: true,
                errorMessage: resolveFilter('campaign.conversion.productisrequired'),
                configuration: {
                    restUrl: application.getData('applicationUrl') + 'cmsapi/CampaignConversionPage',
                    restUrlParams: { objType: 'product' },
                    resultTemplate: resultTemplate,
                    isRequired: true,
                    allowAny: true
                }
            },
            newslettersubscription: {
                selectorLabel: resolveFilter('campaign.conversion.emailcampaignselector'),
                areParametersRequired: true,
                errorMessage: resolveFilter('campaign.conversion.emailcampaignisrequired'),
                configuration: {
                    restUrl: application.getData('applicationUrl') + 'cmsapi/CampaignConversionItem',
                    restUrlParams: { objType: 'newsletter.newsletter' },
                    isRequired: true,
                    allowAny: true
                }
            }
        };

        ctrl.selectedActivity = activityTypesConfiguration.pagevisit;

        $scope.$watch(function () {
            return ctrl.conversion.activityType;
        }, function (newVal, oldVal) {
            ctrl.form.$setPristine();
            ctrl.selectedActivity = activityTypesConfiguration[newVal];
            selectedActivityType = newVal;

            if (newVal === oldVal) {
                return;
            }

            ctrl.selectedConversionItem = {};
        });

        ctrl.confirm = function () {
            ctrl.form.$setSubmitted();
            if (ctrl.form.$valid && ctrl.isSelectionValid()) {

                if (ctrl.selectedConversionItem) {
                    ctrl.conversion.name = ctrl.selectedConversionItem.text || '';
                    ctrl.conversion.itemID = ctrl.selectedConversionItem.id;

                    // Handle the additional 'any' option
                    if (ctrl.selectedConversionItem.isAdditionalOption) {
                        ctrl.conversion.name = '';
                        ctrl.conversion.itemID = null;
                    }

                    // Handle the content only page visit with url parameter
                    if (ctrl.selectedConversionItem.url) {
                        ctrl.conversion.url = ctrl.selectedConversionItem.url;
                        ctrl.conversion.itemID = null;
                   }
                }

                // Check if conversion was updated
                if (!conversionChanged()) {
                    ctrl.dismiss();
                }

                $uibModalInstance.close(ctrl.conversion);
            }
        };

        ctrl.dismiss = function () {
            $uibModalInstance.dismiss();
        };

        ctrl.isSelectionValid = function () {
            return !ctrl.selectedActivity.configuration.isRequired ||
                    ctrl.selectedActivity.configuration.allowAny ||
                    (ctrl.selectedConversionItem && (ctrl.selectedConversionItem.id > 0)) ||
                    (ctrl.selectedConversionItem && ctrl.selectedConversionItem.url && !(ctrl.selectedConversionItem.id > 0));
        }

        function prepareActivityTypes(activityTypes, selectedType) {
            var activityTypesOptions = activityTypes.map(function (activityType) {
                return {
                    id: activityType.type,
                    name: activityType.displayName,
                    selected: selectedType === activityType.type
                }
            });

            return _.sortBy(activityTypesOptions, 'name');
        }

        function conversionChanged() {
            return !conversion
                || (conversion.activityType !== ctrl.conversion.activityType)
                || (conversion.itemID !== ctrl.conversion.itemID);
        }
    }
}(angular, _));
(function (angular) {
    'use strict'

    reportSetupDialogService.$inject = ["$uibModal", "resolveFilter", "serverDataService"];
    angular.module('cms.webanalytics/campaign/reportSetup/reportSetupDialog.service', [
        'cms.webanalytics/campaign/reportSetup/modifyConversion.controller'
    ])
        .service('reportSetupDialogService', reportSetupDialogService);

    /*@ngInject*/
    function reportSetupDialogService($uibModal, resolveFilter, serverDataService) {
        var that = this,
            height = serverDataService.isSiteContentOnly() ? '450px' : '280px';

        that.openDialog = function (conversion, title) {
            return $uibModal.open({
                size: { height: height, width: '600px' },
                templateUrl: 'cms.webanalytics/campaign/reportSetup/dialog/modifyConversion.dialog.html',
                bindToController: true,
                controllerAs: '$ctrl',
                controller: 'ModifyConversionController',
                resolve: {
                    conversion: function () { return conversion },
                    title: function () { return resolveFilter(title) }
                }
            });
        }
    }
}(angular));
(function (angular) {
    'use strict';

    directive.$inject = ["$compile"];
    angular.module('cms.webanalytics/campaign/reportSetup/dialog/selectParameter.directive', [])
        .directive('cmsSelectParameter', directive);

    /*@ngInject*/
    function directive($compile) {
        return {
            restrict: 'E',
            scope: {
                configuration: '=',
                selectedConversion: '='
            },
            templateUrl: 'cms.webanalytics/campaign/reportSetup/dialog/selectParameter.directive.html',
            link: function(scope, element) {
                scope.$watch('configuration', function (newValue, oldValue) {
                    if(!angular.equals(newValue, oldValue)){
                        $compile(element.contents())(scope); 
                    }
                });
            }
        };
    }

}(angular));

(function (angular) {
    'use strict';

    controller.$inject = ["objectiveSetupDialogService", "objectiveService"];
    angular.module('cms.webanalytics/campaign/reportSetup/objective/addObjective.component', [
        'cms.webanalytics/campaign/reportSetup/objective/objectiveSetupDialog.service',
        'cms.webanalytics/campaign/objective.service'
    ])
        .component('cmsAddObjective', addObjective());

    function addObjective() {
        return {
            bindings: {
                objectiveConversions: '<',
                enabled: '<',
                onCreated: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/reportSetup/objective/addObjective.component.html',
            controller: controller,
            replace: true
        };
    }

    /*@ngInject*/
    function controller(objectiveSetupDialogService, objectiveService) {
        var ctrl = this;

        ctrl.openDialog = function () {
            objectiveSetupDialogService.openDialog(ctrl.objectiveConversions, null, 'campaign.objective.defineobjective')
                .then(addObjective);
        };

        function addObjective(objective) {
            objectiveService.addObjective(objective).then(ctrl.onCreated);
        }
    }
}(angular));

(function (angular) {
    'use strict';

    controller.$inject = ["objectiveSetupDialogService", "objectiveService", "confirmationService"];
    angular.module('cms.webanalytics/campaign/reportSetup/objective/editObjective.component', [
        'cms.webanalytics/campaign/reportSetup/objective/objectiveSetupDialog.service',
        'cms.webanalytics/campaign/objective.service',
        'cms.webanalytics/campaign/confirmation.service'
    ])
        .component('cmsEditObjective', component());

    function component() {
        return {
            bindings: {
                objectiveConversions: '<',
                enabled: '<',
                onUpdated: '<',
                objective: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/reportSetup/objective/editObjective.component.html',
            controller: controller,
            replace: true
        };
    }

    /*@ngInject*/
    function controller(objectiveSetupDialogService, objectiveService, confirmationService) {
        var ctrl = this;

        ctrl.openDialog = function () {
            if (confirmationService.canEditObjective()) {
                objectiveSetupDialogService.openDialog(ctrl.objectiveConversions, ctrl.objective, 'campaign.objective.defineobjective')
                    .then(updateObjective);
            }
        };

        function updateObjective(objective) {
            objectiveService.updateObjective(objective).then(ctrl.onUpdated);
        }
    }
}(angular));
(function (angular, _) {
    'use strict';

    controller.$inject = ["confirmationService", "objectiveService"];
    angular.module('cms.webanalytics/campaign/reportSetup/objective.component', [        
        'cms.webanalytics/campaign/reportSetup/objective/addObjective.component',
        'cms.webanalytics/campaign/reportSetup/objective/editObjective.component',
        'cms.webanalytics/campaign/objective.service',
        'CMS/Filters.NumberShortener'
    ])
    .component('cmsCampaignObjective', campaignObjectives());

    function campaignObjectives() {
        return {
            bindings: {
                campaign: '<',
                conversions: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/reportSetup/objective/objective.component.html',
            controller: controller,
            replace: true
        };
    }

    /*@ngInject*/
    function controller(confirmationService, objectiveService) {
        var ctrl = this;

        objectiveService.registerResetObjectiveCallback(resetObjective);
        ctrl.objective = objectiveService.getObjective();
        ctrl.selectedConversionName = getSelectedConversionName(ctrl.objective);       

        ctrl.canAddObjective = function () {
            var hasAnyConversion = _.find(ctrl.conversions, function(conversion) {
                return !conversion.isFunnelStep;
            });
            return !ctrl.campaign.isFinished() && hasAnyConversion && !ctrl.objective;
        }

        ctrl.addObjective = function (objective) {
            ctrl.objective = objective;
            ctrl.selectedConversionName = getSelectedConversionName(objective);
        };

        ctrl.removeObjective = function () {
            if (!ctrl.campaign.isFinished() && ctrl.objective && confirmationService.canRemoveObjective()) {
                objectiveService.removeObjective(ctrl.objective.id).then(ctrl.removeObjectiveFinished);
            }
        };

        ctrl.updateObjective = function (objective) {
            angular.copy(objective, ctrl.objective);
            ctrl.selectedConversionName = getSelectedConversionName(objective);
        }

        ctrl.removeObjectiveFinished = function () {
            ctrl.objective = null;
        };

        function getSelectedConversionName(objective) {

            if (!objective) {
                return null;
            }

            var selectedConversion = _.find(ctrl.conversions, function (conversion) {
                return conversion.id === objective.conversionID;
            });

            var conversionName = selectedConversion.activityName;
            if (selectedConversion.name !== '') {
                conversionName = conversionName + ': ' + selectedConversion.name;
            }

            return conversionName;
        }

        function resetObjective(conversion) {
            if (ctrl.objective && !ctrl.objective.isFunnelStep && ctrl.objective.conversionID === conversion.id) {
                ctrl.objective = null;
            }
        }
    }
}(angular, _));
(function (angular) {
    'use strict';

    controller.$inject = ["$uibModal"];
    modalController.$inject = ["$uibModalInstance", "assetLink", "utmCampaign", "resolveFilter", "getLinkService"];
    angular.module('cms.webanalytics/campaign/inventory/item/getLink.component', [
        'cms.webanalytics/campaign/dialogHeader.component',
        'cms.webanalytics/campaign/dialogFooter.component',
        'cms.webanalytics/campaign/cms.urlHelper.service',
        'cms.webanalytics/campaign/inventory/item/getLink.service'
    ])
        .component('cmsGetLink', getLink());


    function getLink() {
        return {
            bindings: {
                assetLink: '<',
                utmCode: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/inventory/item/getLink/getLink.component.html',
            controller: controller,
            replace: true
        };
    }

    /*@ngInject*/
    function controller($uibModal) {
        var ctrl = this;

        ctrl.openDialog = function () {
            $uibModal.open({
                size: {width: '60%', height: '70%'},
                templateUrl: 'cms.webanalytics/campaign/inventory/item/getLink/getLink.dialog.html',
                bindToController: true,
                controllerAs: '$ctrl',
                controller: modalController,
                resolve: {
                    assetLink: function () { return ctrl.assetLink; },
                    utmCampaign: function () { return ctrl.utmCode; }
                }
            });
        };
    }

    /*@ngInject*/
    function modalController($uibModalInstance, assetLink, utmCampaign, resolveFilter, getLinkService) {
        var ctrl = this;

        ctrl.emptyUtmSourceText = resolveFilter("campaign.getcontentlink.dialog.emptyutmsource");
        ctrl.link = ctrl.emptyUtmSourceText;

        ctrl.dismiss = function () {
            $uibModalInstance.dismiss();
        };

        ctrl.onChange = function () {
            ctrl.link = getLinkService.buildLink(assetLink, utmCampaign, ctrl.utmSource, ctrl.utmMedium, ctrl.utmContent, ctrl.emptyUtmSourceText);
        };

        ctrl.textAreaClick = function (event) {
            event.target.select();
        };
    }
}(angular));

(function (angular, _) {
    'use strict';

    getLinkService.$inject = ["urlHelperService"];
    angular.module('cms.webanalytics/campaign/inventory/item/getLink.service', [
        'cms.webanalytics/campaign/cms.urlHelper.service'
    ])
        .service('getLinkService', getLinkService);

    /*@ngInject*/
    function getLinkService(urlHelperService) {
        var that = this,
            urlHelper = urlHelperService.urlHelper;

        that.buildLink = function (originalLink, utmCampaign, utmSource, utmMedium, utmContent, emptyUtmSourceText) {
            if (!utmSource) {
                return emptyUtmSourceText;
            }

            var originalLinkWithoutQueryString = urlHelper.removeQueryString(originalLink),
                queryParams = urlHelper.getParameters(originalLink);

            if (utmCampaign) {
                queryParams.utm_campaign = utmCampaign;
            }

            queryParams.utm_source = utmSource;

            if (utmMedium) {
                queryParams.utm_medium = utmMedium;
            }

            if (utmContent) {
                queryParams.utm_content = utmContent;
            }

            return originalLinkWithoutQueryString + urlHelper.buildQueryString(queryParams);
        }
    }
}(angular, _));
(function (angular) {
    'use strict';

    controller.$inject = ["reportSetupDialogService", "conversionsService", "conversionsConfigurationService"];
    angular.module('cms.webanalytics/campaign/reportSetup/conversions/addConversion.component', [
        'cms.webanalytics/campaign/conversions.service',
        'cms.webanalytics/campaign/reportSetup/conversionsConfiguration.service',
        'cms.webanalytics/campaign/reportSetup/reportSetupDialog.service'
    ])
        .component('cmsAddConversion', addConversion());

    function addConversion() {
        return {
            bindings: {
                onCreated: '<',
                campaignId: '<',
                enabled: '<',
                isFunnel: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/reportSetup/conversions/conversion/addConversion.component.html',
            controller: controller,
            replace: true
        };
    }

    /*@ngInject*/
    function controller(reportSetupDialogService, conversionsService, conversionsConfigurationService) {
        var ctrl = this;

        ctrl.configuration = conversionsConfigurationService.getConfiguration(ctrl.isFunnel);

        ctrl.openDialog = function () {
            reportSetupDialogService.openDialog(ctrl.conversion, ctrl.configuration.dialogHeading)
                .result.then(addConversion);
        };

        function addConversion (conversion) {
            conversion.isFunnelStep = ctrl.isFunnel;
            conversionsService.addConversion(conversion).then(ctrl.onCreated, undefined);
        }
    }
}(angular));

(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/reportSetup/conversions/conversion.component', [
        'cms.webanalytics/campaign/reportSetup/conversions/editConversion.component',
        'CMS/Filters.Resolve'
    ])
    .component('cmsCampaignConversion', campaignConversion());

    function campaignConversion() {
        return {
            bindings: {
                conversion: '=',
                isFunnel: '<',
                removable: '<',
                editable: '<',
                removeConversion: '&'
            },
            templateUrl: 'cms.webanalytics/campaign/reportSetup/conversions/conversion/conversion.component.html',
            replace: true,
            controller: controller
        };
    }

    function controller() {
        var ctrl = this;

        ctrl.updateConversion = function (conversion) {
            angular.copy(conversion, ctrl.conversion);
        }
    }
}(angular));
(function (angular) {
    'use strict';

    controller.$inject = ["reportSetupDialogService", "conversionsService", "confirmationService", "conversionsConfigurationService"];
    angular.module('cms.webanalytics/campaign/reportSetup/conversions/editConversion.component', [
        'cms.webanalytics/campaign/conversions.service',
        'cms.webanalytics/campaign/reportSetup/conversionsConfiguration.service',
        'cms.webanalytics/campaign/reportSetup/reportSetupDialog.service',
        'cms.webanalytics/campaign/confirmation.service'
    ])
        .component('cmsEditConversion', component());

    /*@ngInject*/
    function component() {
        return {
            bindings: {
                conversion: '<',
                isFunnel: '<',
                enabled: '<',
                onUpdated: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/reportSetup/conversions/conversion/editConversion.component.html',
            controller: controller,
            replace: true
        };
    }

    /*@ngInject*/
    function controller(reportSetupDialogService, conversionsService, confirmationService, conversionsConfigurationService) {
        var ctrl = this;

        ctrl.configuration = conversionsConfigurationService.getConfiguration(ctrl.isFunnel);
        
        ctrl.openDialog = function () {
            if (confirmationService.canEditConversion(ctrl.isFunnel)) {
                reportSetupDialogService.openDialog(ctrl.conversion, ctrl.configuration.dialogHeading)
                    .result.then(updateConversion);
            }
        };

        function updateConversion(conversion) {
            conversionsService.updateConversion(conversion).then(ctrl.onUpdated);
        }
    }
}(angular));
(function (angular) {
    'use strict';

    controller.$inject = ["$uibModalInstance", "objectiveConversions", "objective", "title"];
    angular.module('cms.webanalytics/campaign/reportSetup/objective/modifyObjective.controller', [
        'cms.webanalytics/campaign/cms.application.service'
    ])
        .controller('ModifyObjectiveController', controller);

    /*@ngInject*/
    function controller($uibModalInstance, objectiveConversions, objective, title) {
        var ctrl = this;

        ctrl.objective = angular.copy(objective);

        if (ctrl.objective != null) {
            ctrl.objective.conversionID = String(ctrl.objective.conversionID);
        }

        ctrl.objectiveConversions = prepareObjectiveConversions(objectiveConversions);
        ctrl.title = title;
        ctrl.targetRegexPattern = '^0*[1-9][0-9]*$';

        ctrl.dismiss = function () {
            $uibModalInstance.dismiss();
        };

        ctrl.confirm = function () {
            ctrl.form.$setSubmitted();            
            if (ctrl.form.$valid) {

                if (!objectiveChanged()) {
                    $uibModalInstance.dismiss();
                }

                $uibModalInstance.close(ctrl.objective);
            }
        };

        function objectiveChanged() {
            return !objective
                || (objective.conversionID !== ctrl.objective.conversionID)
                || (objective.value !== ctrl.objective.value);
        }

        function prepareObjectiveConversions(objectiveConversions) {
            var conversions = objectiveConversions.filter(function (conversion) {
                return !conversion.isFunnelStep;
            }).map(function (conversion) {
                var conversionName = conversion.activityName;
                if (conversion.name !== '') {
                    conversionName = conversionName + ': ' + conversion.name;
                }

                return {
                    id: String(conversion.id),
                    name: conversionName
                }
            });

            return conversions;
        }
    }
}(angular));
(function (angular) {
    'use strict';

    objectiveSetupDialogService.$inject = ["$uibModal", "resolveFilter"];
    angular.module('cms.webanalytics/campaign/reportSetup/objective/objectiveSetupDialog.service', [
        'cms.webanalytics/campaign/reportSetup/objective/modifyObjective.controller'
    ])
        .service('objectiveSetupDialogService', objectiveSetupDialogService);

    /*@ngInject*/
    function objectiveSetupDialogService($uibModal, resolveFilter) {
        var that = this;

        that.openDialog = function (objectiveConversions, objective, title) {
            return $uibModal.open({
                size: { height: '400px', width: '600px' },
                templateUrl: 'cms.webanalytics/campaign/reportSetup/objective/dialog/modifyObjective.dialog.html',
                bindToController: true,
                controllerAs: '$ctrl',
                controller: 'ModifyObjectiveController',
                resolve: {
                    objectiveConversions: function () { return objectiveConversions },
                    objective: function () { return objective },
                    title: function () { return resolveFilter(title) }
                }
            }).result;
        }
    }
}(angular));
angular.module("cms.webanalytics/campaign/app.templates", []).run(["$templateCache", function($templateCache) {$templateCache.put("cms.webanalytics/campaign/additionalResources.html","<!-- Resources used in non-html files (resolveFilter(resString)...) so they would not be automagically resolved by Gulp -->\r\n{{ \"general.empty\" | resolve }}\r\n{{ \"general.searching\" | resolve }}\r\n{{ \"general.cancel\" | resolve }}\r\n{{ \"general.save\" | resolve }}\r\n{{ \"general.close\" | resolve }}\r\n{{ \"general.create\" | resolve }}\r\n{{ \"general.noresults\" | resolve }}\r\n{{ \"general.loading.more\" | resolve }}\r\n{{ \"general.loading.error\" | resolve }}\r\n{{ \"smarttip.expand\" | resolve }}\r\n{{ \"general.select\" | resolve }}\r\n{{ \"campaign.autosave.failed\" | resolve }}\r\n{{ \"campaign.autosave.server.error\" | resolve }}\r\n{{ \"campaign.getcontentlink.dialog.emptyutmsource\" | resolve }}\r\n{{ \"campaign.autosave.finished\" | resolve }}\r\n{{ \"campaign.getcontentlink.dialog.title\" | resolve }}\r\n{{ \"documentselection.title\" | resolve }}\r\n{{ \"campaign.assets.failed\" | resolve }}\r\n{{ \"campaign.conversion.failed\" | resolve }}\r\n{{ \"campaign.autosave.validationFailed\" | resolve }}\r\n{{ \"campaign.initialSave.validationAlert\" | resolve }}\r\n{{ \"campaign.create.email\" | resolve }}\r\n{{ \"campaign.create.email.select.type.new\" | resolve }}\r\n{{ \"campaign.create.email.select.type.assign\" | resolve }}\r\n{{ \"campaign.launch.confirm\" | resolve }}\r\n{{ \"campaign.launch.confirm.summary\" | resolve }}\r\n{{ \"campaign.launch.confirm.delimiter\" | resolve }}\r\n{{ \"campaign.launched\" | resolve }}\r\n{{ \"campaign.launched.failed\" | resolve }}\r\n{{ \"campaign.finish.confirm\" | resolve }}\r\n{{ \"campaign.finished\" | resolve }}\r\n{{ \"campaign.finished.failed\" | resolve }}\r\n{{ \"campaign.conversion.productselector\" | resolve }}\r\n{{ \"campaign.conversion.formselector\" | resolve }}\r\n{{ \"campaign.conversion.pageisrequired\" | resolve }}\r\n{{ \"campaign.conversion.formisrequired\" | resolve }}\r\n{{ \"campaign.conversion.productisrequired\" | resolve }}\r\n{{ \"campaign.conversion.eventselector\" | resolve }}\r\n{{ \"campaign.conversion.eventrequired\" | resolve }}\r\n{{ \"campaign.conversion.emailcampaignselector\" | resolve }}\r\n{{ \"campaign.conversion.emailcampaignisrequired\" | resolve }}\r\n{{ \"campaign.message.running\" | resolve }}\r\n{{ \"campaign.message.finished\" | resolve }}\r\n{{ \"campaign.message.scheduled\" | resolve }}\r\n{{ \"campaign.conversions\" | resolve }}\r\n{{ \"campaign.conversions.description\" | resolve }}\r\n{{ \"campaign.conversions.defineconversion\" | resolve }}\r\n{{ \"campaign.conversions.add\" | resolve }}\r\n{{ \"campaign.conversions.add.title\" | resolve }}\r\n{{ \"campaign.journey\" | resolve }}\r\n{{ \"campaign.journey.description\" | resolve }}\r\n{{ \"campaign.journey.definestep\" | resolve }}\r\n{{ \"campaign.journey.add\" | resolve }}\r\n{{ \"campaign.journey.add.title\" | resolve }}\r\n{{ \"campaign.schedule.link\" | resolve }}\r\n<!-- Confirmation messages -->\r\n{{ \"campaign.asset.deleteconfirmation\" | resolve }}\r\n{{ \"campaign.asset.deleteconfirmation.running\" | resolve }}\r\n{{ \"campaign.conversion.deleteconfirmation\" | resolve }}\r\n{{ \"campaign.conversion.deleteconfirmation.running\" | resolve }}\r\n{{ \"campaign.conversion.editconfirmation.running\" | resolve }}\r\n{{ \"campaign.funnelstep.deleteconfirmation\" | resolve }}\r\n{{ \"campaign.funnelstep.deleteconfirmation.running\" | resolve }}\r\n{{ \"campaign.funnelstep.editconfirmation.running\" | resolve }}\r\n{{ \"campaign.objective.deleteconfirmation\" | resolve }}\r\n{{ \"campaign.objective.deleteconfirmation.running\" | resolve }}\r\n{{ \"campaign.objective.editconfirmation.running\" | resolve }}\r\n<!-- Campaign objective -->\r\n{{ \"campaign.objective.defineobjective\" | resolve }}");
$templateCache.put("cms.webanalytics/campaign/campaign.component.html","<div class=\'cms-campaigns-edit form-horizontal\' data-ng-cloak=\'cloak\'>\r\n    <cms-campaign-messages id=\'top\'></cms-campaign-messages>\r\n    <cms-campaign-status-message> </cms-campaign-status-message>\r\n\r\n    <cms-campaign-description campaign=\'$ctrl.campaign\'>\r\n    </cms-campaign-description>\r\n\r\n    <div class=\'campaign-form-category\'>\r\n        <cms-campaign-inventory assets=\'$ctrl.inventoryItems\' site-is-content-only=\'$ctrl.siteIsContentOnly\' url-asset-target-regex=\'$ctrl.urlAssetTargetRegex\' campaign=\'$ctrl.campaign\'>\r\n        </cms-campaign-inventory>\r\n    </div>\r\n    <div class=\'campaign-form-category\'>\r\n        <cms-campaign-promotion items=\'$ctrl.promotionItems\' site-is-content-only=\'$ctrl.siteIsContentOnly\' campaign=\'$ctrl.campaign\'>\r\n        </cms-campaign-promotion>\r\n        <div id=\'promotionSmartTip\'>\r\n            <cms-smart-tip-placeholder smart-tip=\'$ctrl.promotionSmartTip\'>\r\n            </cms-smart-tip-placeholder>\r\n        </div>\r\n    </div>\r\n    <div class=\'campaign-form-category\'>\r\n        <cms-campaign-report-setup conversions=\'$ctrl.conversions\' site-is-content-only=\'$ctrl.siteIsContentOnly\' campaign=\'$ctrl.campaign\'>\r\n        </cms-campaign-report-setup>\r\n    </div>\r\n\r\n    <div id=\'launchSmartTip\' class=\'campaign-form-category\' data-ng-show=\'$ctrl.campaign.isDraft() || $ctrl.campaign.isScheduled()\'>\r\n        <cms-smart-tip-placeholder smart-tip=\'$ctrl.launchSmartTip\' on-ready=\'$ctrl.initLaunchLink()\'>\r\n        </cms-smart-tip-placeholder>\r\n    </div>\r\n</div>");
$templateCache.put("cms.webanalytics/campaign/dialog/dialogFooter.component.html","<div class=\'dialog-footer control-group-inline\'>\r\n    <button class=\'btn btn-default\' type=\'button\' data-ng-show=\'$ctrl.onDismiss\' data-ng-click=\'$ctrl.onDismiss()\'>{{$ctrl.dismissLabel}}</button>\r\n    <button class=\'btn btn-primary\' type=\'button\' data-ng-show=\'$ctrl.onConfirm\' data-ng-click=\'$ctrl.onConfirm()\'>{{$ctrl.confirmLabel}}</button>\r\n</div>\r\n");
$templateCache.put("cms.webanalytics/campaign/dialog/dialogHeader.component.html","<div id=\'dialog-header\' class=\'dialog-header non-selectable\'>\r\n    <div class=\'dialog-header-action-buttons\'>\r\n        <div class=\'action-button\' data-ng-if=\'!$ctrl.maximized\'>\r\n            <a data-ng-click=\'$ctrl.maximize()\'>\r\n                <span class=\'sr-only\'>{{ \"general.maximize\" | resolve }}</span>\r\n                <i class=\'cms-icon-80 icon-modal-maximize\' aria-hidden=\'true\' style=\'cursor: pointer\' title=\'{{ \"general.maximize\" | resolve }}\'></i>\r\n            </a>\r\n        </div>\r\n\r\n        <div class=\'action-button\' data-ng-if=\'$ctrl.maximized\'>\r\n            <a data-ng-click=\'$ctrl.restore()\'>\r\n                <span class=\'sr-only\'>{{ \"general.minimize\" | resolve }}</span>\r\n                <i class=\'cms-icon-80 icon-modal-minimize\' aria-hidden=\'true\' style=\'cursor: pointer\' title=\'{{ \"general.minimize\" | resolve }}\'></i>\r\n            </a>\r\n        </div>\r\n\r\n        <div class=\'action-button close-button\'>\r\n            <a data-ng-click=\'$ctrl.onClose()\'>\r\n                <span class=\'sr-only\'>{{ \"general.close\" | resolve }}</span>\r\n                <i class=\'icon-modal-close cms-icon-150\' aria-hidden=\'true\' style=\'cursor: pointer;\' title=\'{{ \"general.close\" | resolve }}\'></i>\r\n            </a>\r\n        </div>\r\n    </div>\r\n    <h2 class=\'dialog-header-title\'>{{::$ctrl.header}}</h2>\r\n</div>");
$templateCache.put("cms.webanalytics/campaign/description/description.component.html","<div class=\'editing-form-category campaign-category-description\'>\r\n    <form name=\'campaignForm\' cms-autosave=\'$ctrl.saveCampaign()\'>\r\n        <div class=\'campaign-description-textboxes\'>\r\n            <div class=\'campaign-input-padding\' title=\'{{ \"campaign.displayname.description\" | resolve }}\'>\r\n                <cms-campaign-textbox value=\'$ctrl.campaign.displayName\'\r\n                                      maxlength=\'100\'\r\n                                      required=\'true\'\r\n                                      label=\'{{ \"campaign.displayname\" | resolve }}\'\r\n                                      id=\'campaignDisplayName\'\r\n                                      form-element=\'campaignForm.campaignDisplayName\'\r\n                                      enabled=\'$ctrl.campaign.isDraft()\'>\r\n                </cms-campaign-textbox>\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\'campaign-description-textboxes\'>\r\n            <div class=\'campaign-input-padding\' title=\'{{ \"campaign.utmcampaign.description\" | resolve }}\'>\r\n                <cms-campaign-textbox value=\'$ctrl.campaign.utmCode\'\r\n                              maxlength=\'200\'\r\n                              required=\'true\'\r\n                              pattern=\'[0-9a-zA-Z_.-]+\'\r\n                              pattern-error=\'{{ \"campaign.utmparameter.wrongformat\" | resolve }}\'\r\n                              label=\'{{ \"campaign.utmcampaign\" | resolve }}\'\r\n                              id=\'campaignUtmCode\'\r\n                              form-element=\'campaignForm.campaignUtmCode\'\r\n                              enabled=\'$ctrl.campaign.isDraft()\'\r\n                              server-validation-message=\'$ctrl.serverValidationMessage\'>\r\n                </cms-campaign-textbox>\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\'ClearBoth\'></div>\r\n\r\n        <cms-campaign-textarea value=\'$ctrl.campaign.description\'\r\n                               title=\'{{ \"campaign.description.description\" | resolve }}\'\r\n                               rows=\'4\'\r\n                               id=\'campaignDescription\'\r\n                               label=\'{{ \"campaign.description\" | resolve }}\'>\r\n        </cms-campaign-textarea>\r\n    </form>\r\n</div>\r\n");
$templateCache.put("cms.webanalytics/campaign/inventory/inventory.component.html","<div data-ng-cloak=\'cloak\'>\r\n    <h3 class=\'control-label\'>{{ \"campaign.inventory\" | resolve }}</h3>\r\n    <p>{{ \"campaign.inventory.description\" | resolve }}</p>\r\n\r\n    <div class=\'campaign-list\' data-ng-repeat=\'asset in $ctrl.assets track by $index\'>\r\n        <cms-campaign-inventory-item asset=\'asset\' site-is-content-only=\'$ctrl.siteIsContentOnly\' editable=\'!$ctrl.campaign.isFinished()\' utm-code=\'$ctrl.campaign.utmCode\' remove-asset=\'$ctrl.removeAsset(asset)\' data-ng-if=\'asset.id != 0\'>\r\n        </cms-campaign-inventory-item>\r\n    </div>\r\n\r\n    <div class=\'content-block-50\'>\r\n        <div class=\'remove-default-space\' data-ng-if=\'!$ctrl.siteIsContentOnly\'>\r\n            <span>\r\n                <cms-select-page enabled=\'!$ctrl.campaign.isFinished()\' on-select=\'$ctrl.addPageAsset\'>\r\n                </cms-select-page>\r\n            </span>\r\n            <span>\r\n                <cms-create-page enabled=\'!$ctrl.campaign.isFinished()\' on-create=\'$ctrl.addPageAsset\'>\r\n                </cms-create-page>\r\n            </span>\r\n        </div>\r\n        <div data-ng-if=\'$ctrl.siteIsContentOnly\'>\r\n            <cms-add-url-asset on-add=\'$ctrl.addUrlAsset\' campaign=\'$ctrl.campaign\' url-asset-target-regex=\'$ctrl.urlAssetTargetRegex\'>\r\n            </cms-add-url-asset>\r\n        </div>\r\n    </div>\r\n</div>\r\n");
$templateCache.put("cms.webanalytics/campaign/messages/messages.component.html","<div class=\'form-autosave-message alert-error\' data-ng-if=\'$ctrl.getPermanentError()\'>\r\n    {{ $ctrl.getPermanentError() }}\r\n</div>\r\n\r\n<div class=\'form-autosave-message\' data-ng-if=\'$ctrl.temporaryMessage && !$ctrl.getPermanentError()\' data-ng-class=\'$ctrl.temporaryMessageClass\'>\r\n    {{ $ctrl.temporaryMessage }}\r\n</div>\r\n");
$templateCache.put("cms.webanalytics/campaign/reportSetup/reportSetup.component.html","<div data-ng-cloak=\'cloak\'>\r\n    <h3 class=\'control-label\'>{{ \"campaign.reportsetup\" | resolve }}</h3>\r\n\r\n    <!-- Conversions section -->\r\n    <cms-campaign-conversions is-funnel=\'false\' conversions=\'$ctrl.conversions\' site-is-content-only=\'$ctrl.siteIsContentOnly\' campaign=\'$ctrl.campaign\'>\r\n    </cms-campaign-conversions>\r\n\r\n    <!-- Campaign journey section -->\r\n    <cms-campaign-conversions is-funnel=\'true\' conversions=\'$ctrl.conversions\' site-is-content-only=\'$ctrl.siteIsContentOnly\' campaign=\'$ctrl.campaign\'>\r\n    </cms-campaign-conversions>\r\n\r\n    <!-- Campaign objective -->\r\n    <cms-campaign-objective campaign=\'$ctrl.campaign\' conversions=\'$ctrl.conversions\'>\r\n    </cms-campaign-objective>\r\n</div> ");
$templateCache.put("cms.webanalytics/campaign/statusMessage/statusMessage.component.html","<div class=\'alert-dismissable {{$ctrl.alertMessage.cssClass}} alert\' data-ng-show=\'!$ctrl.closed\'>\r\n    <span class=\'alert-icon\'>\r\n        <i class=\'{{$ctrl.alertMessage.icon}}\'></i>\r\n        <span class=\'sr-only\'> {{$ctrl.alertMessage.messageText}}</span>\r\n    </span>\r\n    <div class=\'alert-label\'>\r\n        {{ $ctrl.alertMessage.messageText }}\r\n    </div>\r\n    <span class=\'alert-close\'>\r\n        <i class=\'close icon-modal-close\' data-ng-click=\'$ctrl.close()\'></i>\r\n        <span class=\'sr-only\'>{{ \"general.close\" | resolve }}</span>\r\n    </span>\r\n</div>");
$templateCache.put("cms.webanalytics/campaign/promotion/promotion.component.html","<h3 class=\'control-label\'>{{ \"campaign.promotion\" | resolve }}</h3>\r\n<p>{{ \"campaign.promotion.description\" | resolve }}</p>\r\n\r\n<div class=\'campaign-list\' data-ng-repeat=\'item in $ctrl.items track by $index\'>\r\n    <cms-campaign-promotion-item asset=\'item\' remove-item=\'$ctrl.removeItem(item)\' editable=\'!$ctrl.campaign.isFinished()\'>\r\n    </cms-campaign-promotion-item>\r\n</div>\r\n\r\n<div class=\'content-block-50 remove-default-space\'>\r\n    <cms-select-email on-select=\'$ctrl.addPromotionItem\' enabled=\'!$ctrl.campaign.isFinished()\'>\r\n    </cms-select-email>\r\n\r\n    <cms-create-email items=\'$ctrl.items\' on-create=\'$ctrl.addPromotionItem\' enabled=\'!$ctrl.campaign.isFinished()\'>\r\n    </cms-create-email>\r\n</div>\r\n");
$templateCache.put("cms.webanalytics/campaign/description/textarea/textarea.component.html","<div class=\'campaign-input-padding\' title=\'{{$ctrl.title}}\'>\r\n    <label for=\'{{$ctrl.id}}\' class=\'control-label\'>{{$ctrl.label}}:<span class=\'required-mark\' data-ng-if=\'$ctrl.required\'>*</span></label>\r\n    <textarea class=\'form-control\' id=\'{{$ctrl.id}}\' data-ng-model=\'$ctrl.value\'></textarea>\r\n</div>");
$templateCache.put("cms.webanalytics/campaign/description/textbox/textbox.component.html","<label for=\'{{$ctrl.id}}\' class=\'control-label\'>{{$ctrl.label}}:<span class=\'required-mark\' data-ng-if=\'$ctrl.required\'>*</span></label>\r\n<input type=\'text\' class=\'form-control\' id=\'{{$ctrl.id}}\' name=\'{{$ctrl.id}}\' data-ng-class=\'{ invalid : $ctrl.serverValidationMessage }\' data-ng-model=\'$ctrl.value\' data-ng-pattern=\'$ctrl.pattern\' data-ng-required=\'$ctrl.required\' data-ng-maxlength=\'$ctrl.maxlength\' data-ng-disabled=\'!$ctrl.enabled\'>\r\n\r\n<span class=\'form-control-error\' data-ng-if=\'$ctrl.formElement.$error.pattern\'>{{$ctrl.patternError}}</span>\r\n<span class=\'form-control-error\' data-ng-if=\'$ctrl.formElement.$error.required\'>{{ \"general.requiresvalue\" | resolve }}</span>\r\n<span class=\'form-control-error\' data-ng-if=\'$ctrl.serverValidationMessage && $ctrl.formElement.$valid\'>{{ $ctrl.serverValidationMessage }}</span>");
$templateCache.put("cms.webanalytics/campaign/inventory/addUrlAsset/addUrlAsset.component.html","<button class=\'btn btn-default\' title=\'{{ \"campaignAssetUrl.dialog.title\" | resolve }}\' data-ng-click=\'$ctrl.openDialog()\' data-ng-disabled=\'!$ctrl.campaign.isDraft() || !$ctrl.urlAssetTargetRegex\'>\r\n\r\n    {{ \"campaignAssetUrl.button.title\" | resolve }}\r\n</button>");
$templateCache.put("cms.webanalytics/campaign/inventory/addUrlAsset/addUrlAsset.dialog.html","<cms-dialog-header on-close=\'$ctrl.dismiss\' header=\'{{ \"campaignAssetUrl.dialog.title\" | resolve }}\'></cms-dialog-header>\r\n\r\n<div class=\'dialog-content\'>\r\n    <div class=\'campaign-edit-dialog\'>\r\n        <form class=\'form-horizontal\' name=\'newAssetUrlForm\'>\r\n            <cms-textbox value=\'$ctrl.pageTitle\' maxlength=\'100\'\r\n                         title=\'{{ \"campaignAssetUrl.pageTitle\" | resolve }}\'\r\n                         label=\'{{ \"campaignAssetUrl.pageTitle\" | resolve }}\'\r\n                         required=\'true\'\r\n                         explanation-text=\'{{ \"campaignAssetUrl.pageTitle.explanationText\" | resolve }}\'\r\n                         id=\'pageTitle\'>\r\n\r\n            </cms-textbox>\r\n            <cms-textarea value=\'$ctrl.target\' maxlength=\'4096\' required=\'true\'\r\n                          title=\'{{ \"campaignAssetUrl.target\" | resolve }}\'\r\n                          regex-pattern=\'{{::$ctrl.urlAssetTargetRegex}}\'\r\n                          regex-pattern-modifiers=\'i\'\r\n                          regex-pattern-error=\'{{ \"campaignAssetUrl.target.wrongFormat\" | resolve }}\'\r\n                          label=\'{{ \"campaignAssetUrl.target\" | resolve }}\'\r\n                          explanation-text=\'{{ \"campaignAssetUrl.target.explanationText\" | resolve }}\'\r\n                          id=\'url\'\r\n                          placeholder=\'https://example.com/page\'> \r\n            </cms-textarea> \r\n        </form>\r\n    </div>\r\n</div>\r\n<cms-dialog-footer confirm-label=\'{{ \"general.saveandclose\" | resolve }}\' on-confirm=\'$ctrl.confirm\'></cms-dialog-footer>");
$templateCache.put("cms.webanalytics/campaign/inventory/createPage/createPage.component.html","<cms-create-page-dialog change=\'$ctrl.dataChange()\' click=\'$ctrl.dataClick()\' id=\'campaignAssetNewLandingPage\' button-id=\'campaignAssetCreatePageButton\' dialog-name=\'newpage\' dialog-url=\'{{$ctrl.dialogUrl}}\' dialog-width=\'90%\' dialog-height=\'90%\' text=\'{{ \"campaign.assets.content.create\" | resolve }}\' title=\'{{ \"campaign.assets.content.create.description\" | resolve }}\' data-disabled=\'!$ctrl.enabled\' data-ng-model=\'$ctrl.newPageAsset\'>\r\n</cms-create-page-dialog>");
$templateCache.put("cms.webanalytics/campaign/inventory/item/item.component.html","<div class=\'campaign-assetlist-item row\'>\r\n    <div class=\'campaign-assetlist-header\'>\r\n        <span class=\'campaign-assetlist-header-description\'>\r\n            <span class=\'primary-description\' data-ng-if=\'$ctrl.isExistingDocument()\'>\r\n                {{$ctrl.asset.additionalProperties.documentType}}\r\n                <br>\r\n            </span>\r\n\r\n            <span data-ng-if=\'!$ctrl.isDeleted()\'><a target=\'_blank\' data-ng-href=\'{{$ctrl.asset.link}}\'>{{$ctrl.asset.name}}</a></span>\r\n            <span class=\'campaign-assetlist-deleted\' data-ng-if=\'$ctrl.isDeleted()\'>{{$ctrl.asset.name}}</span>\r\n\r\n            <span data-ng-if=\'!$ctrl.siteIsContentOnly && $ctrl.isExistingDocument()\'>\r\n                <i class=\'campaign-assetlist-header-icon icon-check-circle color-green-100\' title=\'{{ \"tree.publishednodetooltip\" | resolve }}\' aria-hidden=\'true\' data-ng-if=\'$ctrl.isPublished()\'>\r\n                </i>\r\n                <i class=\'campaign-assetlist-header-icon icon-times-circle color-red-70\' title=\'{{ \"tree.unpublishednodetooltip\" | resolve }}\' aria-hidden=\'true\' data-ng-if=\'!$ctrl.isPublished()\'>\r\n                </i>\r\n            </span>\r\n        </span>\r\n    </div>\r\n\r\n    <div class=\'campaign-assetlist-icons\'>\r\n        <div title=\'{{ \"campaign.assetlist.removeasset\" | resolve }}\'>\r\n            <button type=\'button\' value=\'\' class=\'icon-only btn-icon btn\' title=\'{{ \"campaign.assetlist.removeasset\" | resolve }}\' data-ng-disabled=\'!$ctrl.editable\' data-ng-click=\'$ctrl.removeAsset()\'>\r\n                <i aria-hidden=\'true\' class=\'icon-bin\'></i>\r\n                <span class=\'sr-only\'>{{ \"campaign.assetlist.removeasset\" | resolve }}</span>\r\n            </button>\r\n        </div>\r\n\r\n        <div title=\'{{ \"general.edit\" | resolve }}\' data-ng-if=\'!$ctrl.isDeleted()\'>\r\n            <a target=\'_blank\' class=\'btn icon-only btn-icon\' data-ng-href=\'{{$ctrl.asset.link}}\'>\r\n                <i class=\'icon-edit\' aria-hidden=\'true\'>\r\n                </i>\r\n            </a>\r\n        </div>\r\n\r\n        <cms-get-link asset-link=\'$ctrl.asset.additionalProperties.liveSiteLink\' utm-code=\'$ctrl.utmCode\' data-ng-if=\'$ctrl.asset.id > 0\'></cms-get-link>\r\n    </div>\r\n</div>");
$templateCache.put("cms.webanalytics/campaign/inventory/selectPage/selectPage.component.html","<button class=\'btn btn-default\' title=\'{{ \"campaign.assets.landingpage.description\" | resolve }}\' data-ng-click=\'$ctrl.openDialog()\' data-ng-disabled=\'!$ctrl.enabled\'>\r\n    {{ \"campaign.assets.content.add\" | resolve }}\r\n</button>");
$templateCache.put("cms.webanalytics/campaign/inventory/selectPage/selectPage.dialog.html","<div id=\'js-page-selector-container\'>\r\n    <cms-dialog-header on-close=\'$ctrl.dismiss\' header=\'{{ \"documentselection.title\" | resolve }}\'></cms-dialog-header>\r\n    <div class=\'dialog-content\'>\r\n        <form class=\'form-horizontal\' name=\'$ctrl.form\' novalidate=\'\'>\r\n            <div class=\'editing-form-label-cell\'>\r\n                <label for=\'pageSelector\' class=\'control-label editing-form-label\'>\r\n                    {{ \"campaign.conversion.pageselector\" | resolve }}<span class=\'required-mark\'>*</span>\r\n                </label>\r\n            </div>\r\n\r\n            <div class=\'editing-form-value-cell\'>\r\n                <div class=\'editing-form-control-nested-control\'>\r\n                    <select id=\'pageSelector\' name=\'pageSelector\' class=\'form-control\' model-item=\'$ctrl.detail\' result-template=\'$ctrl.resultTemplate\' rest-url=\'{{$ctrl.restUrl}}\' rest-url-params=\'$ctrl.restUrlParams\' dropdown-parent-id=\'js-page-selector-container\' cms-select2=\'\' data-ng-min=\'1\'>\r\n                     </select>\r\n                </div>\r\n                <span class=\'form-control-error\' data-ng-if=\'!$ctrl.isSelectionValid() && ($ctrl.form.pageSelector.$touched || $ctrl.form.$submitted);\'>\r\n                    {{ \"campaign.conversion.pageisrequired\" | resolve }}\r\n                </span>\r\n            </div>\r\n        </form>\r\n    </div>\r\n    <cms-dialog-footer confirm-label=\'{{ \"general.save\" | resolve }}\' on-confirm=\'$ctrl.confirm\' dismiss-label=\'{{ \"general.cancel\" | resolve }}\' on-dismiss=\'$ctrl.dismiss\'></cms-dialog-footer>\r\n</div>");
$templateCache.put("cms.webanalytics/campaign/reportSetup/conversions/conversions.component.html","<div>\r\n    <h4 class=\'control-label {{ $ctrl.configuration.additionalClass }}\'>{{ $ctrl.configuration.heading }}</h4>\r\n    <p>{{ $ctrl.configuration.description }}</p>\r\n\r\n    <div class=\'campaign-list\' data-ng-repeat=\'conversion in $ctrl.conversions | filter: { isFunnelStep: $ctrl.isFunnel }\'>\r\n        <cms-campaign-conversion is-funnel=\'$ctrl.isFunnel\' conversion=\'conversion\' removable=\'$ctrl.isRemovable()\' editable=\'!$ctrl.campaign.isFinished()\' remove-conversion=\'$ctrl.removeConversion(conversion)\'>\r\n        </cms-campaign-conversion>\r\n    </div>\r\n\r\n    <div class=\'content-block-50\'>\r\n        <cms-add-conversion is-funnel=\'$ctrl.isFunnel\' on-created=\'$ctrl.addConversionToList\' enabled=\'!$ctrl.campaign.isFinished()\' campaign-id=\'$ctrl.campaign.campaignID\'>\r\n        </cms-add-conversion>\r\n    </div>\r\n</div>");
$templateCache.put("cms.webanalytics/campaign/reportSetup/dialog/modifyConversion.dialog.html","<div id=\'js-page-selector-container\'>\r\n    <cms-dialog-header on-close=\'$ctrl.dismiss\' header=\'{{ $ctrl.title }}\'></cms-dialog-header>\r\n    <div class=\'dialog-content\'>\r\n        <div class=\'campaign-edit-dialog\'>\r\n            <form class=\'form-horizontal\' name=\'$ctrl.form\'>\r\n                <cms-select value=\'$ctrl.conversion.activityType\' options=\'$ctrl.activityTypes\' title=\'{{ \"campaign.conversion.visitoractivity\" | resolve }}\' label=\'{{ \"campaign.conversion.visitoractivity\" | resolve }}\' id=\'selectActivityType\' required=\'true\' info-text=\'{{ \"campaign.conversion.visitoractivityinfo\" | resolve }}\'>\r\n                </cms-select>\r\n\r\n                <div class=\'form-group\' data-ng-if=\'$ctrl.selectedActivity.areParametersRequired\'>\r\n                    <div class=\'editing-form-label-cell\'>\r\n                        <label for=\'pageSelector\' class=\'control-label editing-form-label\'>\r\n                            {{ $ctrl.selectedActivity.selectorLabel }}<span class=\'required-mark\' data-ng-if=\'$ctrl.selectedActivity.configuration.isRequired\'>*</span>\r\n                        </label>\r\n                    </div>\r\n\r\n                    <div class=\'editing-form-value-cell\'>\r\n                        <cms-select-parameter configuration=\'$ctrl.selectedActivity.configuration\' selected-conversion=\'$ctrl.selectedConversionItem\'>\r\n                        </cms-select-parameter>\r\n\r\n                        <span class=\'form-control-error\' data-ng-if=\'!$ctrl.isSelectionValid() &&\r\n                              ($ctrl.form.pageSelector.$touched || $ctrl.form.$submitted);\'>\r\n\r\n                            {{ $ctrl.selectedActivity.errorMessage}}\r\n                        </span>\r\n                    </div>\r\n                </div>\r\n\r\n                <!-- Page visit selector for content only sites -->\r\n                <div data-ng-if=\'$ctrl.showContentOnlyPageVisitConfiguration()\'>\r\n                    <cms-textarea value=\'$ctrl.selectedConversionItem.url\' maxlength=\'4096\' required=\'true\' title=\'{{ \"campaign.conversion.pagevisit.pageurl\" | resolve }}\' regex-pattern=\'{{ ::$ctrl.urlAssetTargetRegex }}\' regex-pattern-modifiers=\'i\' regex-pattern-error=\'{{ \"campaignAssetUrl.target.wrongFormat\" | resolve }}\' label=\'{{ \"campaign.conversion.pagevisit.pageurl\" | resolve }}\' id=\'pageURL\' placeholder=\'https://example.com/page\'>\r\n                    </cms-textarea>\r\n\r\n                    <cms-textbox value=\'$ctrl.selectedConversionItem.text\' maxlength=\'4096\' required=\'true\' title=\'{{ \"campaign.conversion.pagevisit.pagename\" | resolve }}\' label=\'{{ \"campaign.conversion.pagevisit.pagename\" | resolve }}\' explanation-text=\'{{ \"campaign.conversion.pagevisit.explanationText\" | resolve }}\' id=\'pageName\'>\r\n                    </cms-textbox> \r\n                </div>\r\n            </form>\r\n        </div>\r\n    </div>\r\n    <cms-dialog-footer confirm-label=\'{{ \"general.save\" | resolve }}\' on-confirm=\'$ctrl.confirm\' dismiss-label=\'{{ \"general.cancel\" | resolve }}\' on-dismiss=\'$ctrl.dismiss\'></cms-dialog-footer>\r\n</div>");
$templateCache.put("cms.webanalytics/campaign/reportSetup/dialog/selectParameter.directive.html","<div class=\'editing-form-control-nested-control\'>\r\n    <select id=\'pageSelector\' name=\'pageSelector\' class=\'form-control\'\r\n            model-item=\'selectedConversion\'\r\n            result-template=\'configuration.resultTemplate\'\r\n            rest-url=\'{{configuration.restUrl}}\'\r\n            rest-url-params=\'configuration.restUrlParams\' \r\n            dropdown-parent-id=\'js-page-selector-container\'\r\n            show-additional-option=\'configuration.allowAny\'\r\n            additional-option-text=\'{{ \"general.selectany\" | resolve }}\'\r\n            cms-select2> \r\n    </select>\r\n</div>\r\n");
$templateCache.put("cms.webanalytics/campaign/reportSetup/objective/addObjective.component.html","<button class=\'btn btn-default\' title=\'{{ \"campaign.objective.add.title\" | resolve }}\' data-ng-click=\'$ctrl.openDialog()\' data-ng-disabled=\'!$ctrl.enabled\'>    \r\n    {{ \"campaign.objective.add\" | resolve }}\r\n</button>");
$templateCache.put("cms.webanalytics/campaign/reportSetup/objective/editObjective.component.html","<div>\r\n    <button title=\'{{ \"general.edit\" | resolve }}\' class=\'icon-only btn-icon btn\' data-ng-disabled=\'!$ctrl.enabled\' data-ng-click=\'$ctrl.openDialog()\'>\r\n        <i aria-hidden=\'true\' class=\'icon-edit\'></i>\r\n    </button>\r\n</div>");
$templateCache.put("cms.webanalytics/campaign/reportSetup/objective/objective.component.html"," <div>\r\n    <h4 class=\'control-label separator\'>{{ \"campaign.objective\" | resolve }}</h4>\r\n    <p>{{ \"campaign.objective.description\" | resolve }}</p>\r\n\r\n    <div class=\'campaign-list\' data-ng-if=\'$ctrl.objective\'>\r\n        <div class=\'campaign-assetlist-item row\'>\r\n            <div class=\'campaign-assetlist-header\'>\r\n                <span class=\'campaign-assetlist-header-description\'>\r\n                    <span class=\'primary-description\'>\r\n                        {{ \"campaign.objective\" | resolve }}\r\n                    </span>\r\n                    <span>\r\n                        <br>\r\n                        {{ $ctrl.selectedConversionName }}\r\n                    </span>\r\n                </span>\r\n            </div>\r\n            <div class=\'campaign-assetlist-icons\'>                \r\n                <div>\r\n                    <button title=\'{{ \"general.remove\" | resolve }}\' class=\'icon-only btn-icon btn\' data-ng-disabled=\'$ctrl.campaign.isFinished()\' data-ng-click=\'$ctrl.removeObjective()\'>\r\n                        <i aria-hidden=\'true\' class=\'icon-bin\'></i>\r\n                    </button>\r\n                </div>\r\n                <cms-edit-objective objective=\'$ctrl.objective\' objective-conversions=\'$ctrl.conversions\' enabled=\'!$ctrl.campaign.isFinished()\' on-updated=\'$ctrl.updateObjective\'>\r\n                </cms-edit-objective>\r\n                <div class=\'additional-info\'>\r\n                    <span class=\'primary-text\'>{{ $ctrl.objective.value | shortNumber }}</span>\r\n                    <br>\r\n                    <span>{{ \"campaign.objective.target\" | resolve }}</span>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n    <div class=\'content-block-50\'>\r\n        <cms-add-objective enabled=\'$ctrl.canAddObjective()\' objective-conversions=\'$ctrl.conversions\' on-created=\'$ctrl.addObjective\' data-ng-if=\'!$ctrl.objective\'>\r\n        </cms-add-objective>\r\n    </div>\r\n</div>\r\n");
$templateCache.put("cms.webanalytics/campaign/promotion/createEmail/createEmail.component.html","<button class=\'btn btn-default\' title=\'{{ \"campaign.create.email.description\" | resolve }}\' data-ng-click=\'$ctrl.openDialog()\' data-ng-disabled=\'!$ctrl.enabled\'>\r\n    {{ \"campaign.create.email\" | resolve }}\r\n</button>");
$templateCache.put("cms.webanalytics/campaign/promotion/createEmail/createEmail.dialog.html","<cms-dialog-header on-close=\'$ctrl.dismiss\' header=\'{{ \"campaign.create.email\" | resolve }}\'></cms-dialog-header>\r\n<div class=\'dialog-content\'>\r\n    <div class=\'campaign-edit-dialog\'>\r\n        <form class=\'form-horizontal\' name=\'newEmailForm\'>\r\n            <cms-textbox value=\'$ctrl.emailSubject\' maxlength=\'100\' required=\'true\' title=\'{{ \"campaign.create.email.subject\" | resolve }}\' label=\'{{ \"campaign.create.email.subject\" | resolve }}\' id=\'email-subject\'>\r\n            </cms-textbox>\r\n\r\n            <cms-radio-button value=\'$ctrl.emailCampaignType\' options=\'$ctrl.emailCampaignTypeOptions\' name=\'assignOrCreateEmailCampaign\' id=\'email-campaign-select\'>\r\n            </cms-radio-button>\r\n\r\n            <div data-ng-show=\'$ctrl.isNewCampaignType()\'>\r\n                <cms-textbox value=\'$ctrl.emailDisplayName\' maxlength=\'100\' required=\'$ctrl.isNewCampaignType()\' title=\'{{ \"newsletter_edit.newsletterdisplaynamelabel\" | resolve }}\' label=\'{{ \"newsletter_edit.newsletterdisplaynamelabel\" | resolve }}\' id=\'email-display-name\'>\r\n                </cms-textbox>\r\n\r\n                <cms-textbox value=\'$ctrl.emailSenderName\' maxlength=\'100\' required=\'$ctrl.isNewCampaignType()\' title=\'{{ \"newsletter_edit.newslettersendernamelabel\" | resolve }}\' label=\'{{ \"newsletter_edit.newslettersendernamelabel\" | resolve }}\' id=\'email-sender-name\'>\r\n                </cms-textbox>\r\n\r\n                <cms-textbox value=\'$ctrl.emailSenderAddress\' input-type=\'email\' pattern=\'$ctrl.emailPattern\' pattern-error=\'{{ \"campaign.create.email.campaign.senderaddress.invalid.email\" | resolve }}\' maxlength=\'100\' required=\'$ctrl.isNewCampaignType()\' title=\'{{ \"newsletter_edit.newslettersenderemaillabel\" | resolve }}\' label=\'{{ \"newsletter_edit.newslettersenderemaillabel\" | resolve }}\' id=\'email-sender-address\'>\r\n                </cms-textbox>\r\n\r\n                <cms-select value=\'$ctrl.templateUnsubscription\' options=\'$ctrl.data.emailTemplates.unsubscription\' required=\'$ctrl.isNewCampaignType()\' title=\'{{ \"newsletter_edit.unsubscriptiontemplate\" | resolve }}\' label=\'{{ \"newsletter_edit.unsubscriptiontemplate\" | resolve }}\' id=\'template-unsubscription\'>\r\n                </cms-select>\r\n\r\n                <cms-select value=\'$ctrl.templateIssue\' options=\'$ctrl.data.emailTemplates.issue\' required=\'$ctrl.isNewCampaignType()\' title=\'{{ \"newsletter_edit.newslettertemplate\" | resolve }}\' label=\'{{ \"newsletter_edit.newslettertemplate\" | resolve }}\' id=\'template-issue\'>\r\n                </cms-select>\r\n            </div>\r\n\r\n            <div data-ng-show=\'!$ctrl.isNewCampaignType()\'>\r\n                <div title=\'{{ \"newsletter_newsletter.select.selectitem\" | resolve }}\' class=\'form-group\'>\r\n                    <div class=\'editing-form-label-cell\'>\r\n                        <label for=\'template-issue\' class=\'control-label editing-form-label\'>\r\n                            {{ \"newsletter_newsletter.select.selectitem\" | resolve }}:<span class=\'required-mark\'>*</span>\r\n                        </label>\r\n                    </div>\r\n                    <div class=\'editing-form-value-cell\'>\r\n                        <div class=\'editing-form-control-nested-control\'>\r\n                            <select class=\'form-control\' id=\'email-newsletter-select\' data-ng-model=\'$ctrl.emailCampaignSelect\' data-ng-required=\'!$ctrl.isNewCampaignType()\' name=\'emailNewsletterSelect\' data-ng-options=\'campaign as campaign.name for campaign in $ctrl.data.emailCampaigns track by campaign.id\' data-ng-change=\'templatesSelector = $ctrl.emailCampaignSelect.templates\'></select>\r\n                            <span class=\'form-control-error\' data-ng-show=\'(newEmailForm.$submitted || $ctrl.emailCampaignSelect.$dirty) && newEmailForm.emailNewsletterSelect.$error.required\'>\r\n                                {{\'general.requiresvalue\'|resolve}}\r\n                            </span>\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n\r\n                <div title=\'{{ \"newsletter_edit.newslettertemplate\" | resolve }}\' class=\'form-group\'>\r\n                    <div class=\'editing-form-label-cell\'>\r\n                        <label for=\'email-template-select\' class=\'control-label editing-form-label\'>\r\n                            {{ \"newsletter_edit.newslettertemplate\" | resolve }}:<span class=\'required-mark\'>*</span>\r\n                        </label>\r\n                    </div>\r\n                    <div class=\'editing-form-value-cell\'>\r\n                        <div class=\'editing-form-control-nested-control\'>\r\n                            <select class=\'form-control\' id=\'email-template-select\' data-ng-model=\'$ctrl.templateIssue\' data-ng-required=\'!$ctrl.isNewCampaignType()\' name=\'emailTemplateSelect\' data-ng-options=\'template as template.displayName for template in templatesSelector track by template.id\'></select>\r\n                            <span class=\'form-control-error\' data-ng-show=\'(newEmailForm.$submitted || $ctrl.templateIssue.$dirty) && newEmailForm.emailTemplateSelect.$error.required\'>\r\n                                {{\'general.requiresvalue\'|resolve}}\r\n                            </span>\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n        </form>\r\n    </div>\r\n</div>\r\n<cms-dialog-footer confirm-label=\'{{ \"general.create\" | resolve }}\' on-confirm=\'$ctrl.confirm\'></cms-dialog-footer>");
$templateCache.put("cms.webanalytics/campaign/promotion/promotionItem/promotionItem.component.html","<div class=\'campaign-promotionlist-item row form-horizontal\'>\r\n    <div class=\'col-xs-6\'>\r\n        <div class=\'campaign-assetlist-header\'>\r\n            <span class=\'campaign-assetlist-header-description\'>\r\n                <span class=\'primary-description\'>\r\n                    {{ \"campaign.assetlist.email\" | resolve }}\r\n                    <br>\r\n                </span>\r\n\r\n                <span data-ng-if=\'!$ctrl.isDeleted()\'><a target=\'_blank\' data-ng-href=\'{{$ctrl.asset.link}}\'>{{$ctrl.asset.name}}</a> </span>\r\n                <span class=\'campaign-assetlist-deleted\' data-ng-if=\'$ctrl.isDeleted()\'>{{$ctrl.asset.name}}</span>\r\n            </span>\r\n        </div>\r\n    </div>\r\n\r\n    <div class=\'campaign-email-source col-xs-4\' title=\'{{ \"campaign.assetlist.email\" | resolve }}\'>\r\n        <form name=\'{{$ctrl.formName}}\' cms-autosave=\'$ctrl.save()\'>\r\n            <cms-campaign-textbox value=\'$ctrl.asset.additionalProperties.utmSource\' maxlength=\'\\\'200\\\'\' required=\'true\' pattern=\'[0-9a-zA-Z_.-]+\' pattern-error=\'{{ \"campaign.utmparameter.wrongformat\" | resolve }}\' label=\'{{ \"campaign.utmsource\" | resolve }}\' id=\'{{$ctrl.inputId}}\' enabled=\'$ctrl.editable && $ctrl.asset.additionalProperties.isEditable\' form-element=\'$ctrl.getUtmSourceField()\'>\r\n            </cms-campaign-textbox>\r\n        </form>\r\n    </div>\r\n\r\n    <div class=\'cols-xs-2 promotionlist-icons\'>\r\n        <div title=\'{{ \"general.edit\" | resolve }}\' data-ng-if=\'!$ctrl.isDeleted()\'>\r\n            <a target=\'_blank\' class=\'btn icon-only btn-icon\' data-ng-href=\'{{$ctrl.asset.link}}\'>\r\n                <i class=\'icon-edit\' aria-hidden=\'true\'>\r\n                </i>\r\n            </a>\r\n        </div>\r\n\r\n        <button class=\'icon-only btn-icon btn\' title=\'{{ \"campaign.assetlist.removeemail\" | resolve }}\' data-ng-click=\'$ctrl.removeItem()\' data-ng-disabled=\'!$ctrl.editable\'>\r\n            <i class=\'icon-bin\' aria-hidden=\'true\'></i>\r\n            <span class=\'sr-only\'>{{ \"campaign.assetlist.removeemail\" | resolve }}</span>\r\n        </button>\r\n\r\n    </div>\r\n\r\n</div>");
$templateCache.put("cms.webanalytics/campaign/promotion/selectEmail/selectEmail.component.html","<cms-select-item-dialog change=\'$ctrl.onChange()\' click=\'$ctrl.onClick()\' id=\'campaignAssetEmail\' button-id=\'campaignAssetEmailButton\' dialog-name=\'ItemSelection\' dialog-url=\'{{$ctrl.dialogUrl}}\' dialog-width=\'700px\' dialog-height=\'725px\' title=\'{{ \"campaign.assets.email.description\" | resolve }}\' text=\'{{ \"campaign.assets.email.add\" | resolve }}\' data-disabled=\'!$ctrl.enabled\' data-ng-model=\'$ctrl.newEmailAsset\'>\r\n</cms-select-item-dialog>");
$templateCache.put("cms.webanalytics/campaign/inventory/item/getLink/getLink.component.html","<div class=\'campaign-assetlist-link\' title=\'{{ \"campaign.getcontentlink.button.title\" | resolve }}\'>\r\n    <a title=\'{{ \"campaign.getcontentlink.button.title\" | resolve }}\' data-ng-click=\'$ctrl.openDialog()\'>\r\n        {{ \"campaign.getcontentlink.urlbuilder\" | resolve }}\r\n    </a>\r\n</div>");
$templateCache.put("cms.webanalytics/campaign/inventory/item/getLink/getLink.dialog.html","<cms-dialog-header on-close=\'$ctrl.dismiss\' header=\'{{ \"campaign.getcontentlink.dialog.title\" | resolve }}\'></cms-dialog-header>\r\n<div class=\'dialog-content\'>\r\n    <div class=\'campaign-edit-dialog\'>\r\n        <p class=\'form-horizontal\'>{{ \"campaign.getcontentlink.dialog.description\" | resolve }}</p>\r\n\r\n        <form class=\'form-horizontal\' name=\'getLinkForm\'>\r\n            <div class=\'form-group\' title=\'{{ \"campaign.utmsource\" | resolve }}\'>\r\n                <div class=\'editing-form-label-cell\'>\r\n                    <label class=\'control-label editing-form-label\' for=\'utmSource\'>\r\n                        {{ \"campaign.utmsource\" | resolve }}:\r\n                        <span class=\'required-mark\'>*</span>\r\n                    </label>\r\n                </div>\r\n                <div class=\'editing-form-value-cell\'>\r\n                    <div class=\'editing-form-control-nested-control\'>\r\n                        <input name=\'utmSource\' id=\'utmSource\' class=\'form-control\' type=\'text\' pattern=\'[0-9a-zA-Z_.-]+\' maxlength=\'100\' required data-ng-maxlength=\'100\' data-ng-change=\'$ctrl.onChange()\' data-ng-model=\'$ctrl.utmSource\'>\r\n                        <div class=\'explanation-text\'>{{ \"campaign.getcontentlink.dialog.utmsource.explanationtext\" | resolve }}</div>\r\n                        <span class=\'form-control-error\' data-ng-if=\'getLinkForm.utmSource.$error.pattern\'>{{ \"campaign.utmparameter.wrongformat\" | resolve }}</span>\r\n                        <span class=\'form-control-error\' data-ng-if=\'getLinkForm.utmSource.$error.required && getLinkForm.utmSource.$touched\'>{{ \"general.requiresvalue\" | resolve }}</span>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n            <div class=\'form-group\' title=\'{{ \"campaign.utmmedium\" | resolve }}\'>\r\n                <div class=\'editing-form-label-cell\'>\r\n                    <label class=\'control-label editing-form-label\' for=\'utmMedium\'>{{ \"campaign.utmmedium\" | resolve }}:</label>\r\n                </div>\r\n                <div class=\'editing-form-value-cell\'>\r\n                    <div class=\'editing-form-control-nested-control\'>\r\n                        <input name=\'utmMedium\' id=\'utmMedium\' class=\'form-control\' type=\'text\' pattern=\'[0-9a-zA-Z_.-]+\' maxlength=\'100\' data-ng-maxlength=\'100\' data-ng-model=\'$ctrl.utmMedium\' data-ng-change=\'$ctrl.onChange()\'>\r\n                        <div class=\'explanation-text\'>{{ \"campaign.getcontentlink.dialog.utmmedium.explanationtext\" | resolve }}</div>\r\n                        <span class=\'form-control-error\' data-ng-if=\'getLinkForm.utmMedium.$error.pattern\'>{{ \"campaign.utmparameter.wrongformat\" | resolve }}</span>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n            <div class=\'form-group\' title=\'{{ \"campaign.utmcontent\" | resolve }}\'>\r\n                <div class=\'editing-form-label-cell\'>\r\n                    <label class=\'control-label editing-form-label\' for=\'utmContent\'>{{ \"campaign.utmcontent\" | resolve }}:</label>\r\n                </div>\r\n                <div class=\'editing-form-value-cell\'>\r\n                    <div class=\'editing-form-control-nested-control\'>\r\n                        <input name=\'utmContent\' id=\'utmContent\' class=\'form-control\' type=\'text\' pattern=\'[0-9a-zA-Z_.-]+\' maxlength=\'100\' data-ng-maxlength=\'100\' data-ng-model=\'$ctrl.utmContent\' data-ng-change=\'$ctrl.onChange()\'>\r\n                        <div class=\'explanation-text\'>{{ \"campaign.getcontentlink.dialog.utmcontent.explanationtext\" | resolve }}</div>\r\n                        <span class=\'form-control-error\' data-ng-if=\'getLinkForm.utmContent.$error.pattern\'>{{ \"campaign.utmparameter.wrongformat\" | resolve }}</span>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n            <div class=\'ng-isolate-scope form-group\' title=\'{{ \"campaign.getcontentlink.dialog.linktextarea.label\" | resolve }}\'>\r\n                <div class=\'editing-form-label-cell\'>\r\n                    <label class=\'control-label editing-form-label\' for=\'get-link-dialog-built-link\'>{{ \"campaign.getcontentlink.dialog.linktextarea.label\" | resolve }}:</label>\r\n                </div>\r\n                <div class=\'textarea editing-form-value-cell\'>\r\n                    <textarea id=\'get-link-dialog-built-link\' class=\'form-control\' placeholder=\'\' required=\'required\' name=\'get-link-dialog-built-link\' rows=\'4\' readonly data-ng-click=\'$ctrl.textAreaClick($event)\' data-ng-model=\'$ctrl.link\'></textarea>\r\n                </div>\r\n            </div>\r\n        </form>\r\n    </div>\r\n</div>\r\n<cms-dialog-footer dismiss-label=\'{{ \"general.close\" | resolve }}\' on-dismiss=\'$ctrl.dismiss\'></cms-dialog-footer>");
$templateCache.put("cms.webanalytics/campaign/reportSetup/conversions/conversion/addConversion.component.html","<button class=\'btn btn-default\' title=\'{{ $ctrl.configuration.addButtonTitle }}\' data-ng-disabled=\'!$ctrl.enabled\' data-ng-click=\'$ctrl.openDialog()\'>\r\n    {{ $ctrl.configuration.addButtonLabel }}\r\n</button>");
$templateCache.put("cms.webanalytics/campaign/reportSetup/conversions/conversion/conversion.component.html","<div class=\'campaign-assetlist-item row\'>\r\n    <div class=\'campaign-assetlist-header\'>\r\n        <span class=\'campaign-assetlist-header-description\'>\r\n            <span class=\'primary-description\'>\r\n                {{$ctrl.conversion.activityName}}\r\n            </span>\r\n            <span data-ng-if=\'$ctrl.conversion.name && $ctrl.conversion.name.length > 0\'>\r\n                <br>\r\n                {{$ctrl.conversion.name}}\r\n            </span>\r\n        </span>\r\n    </div>\r\n    <div class=\'campaign-assetlist-icons\'>\r\n        <div>\r\n            <button title=\'{{ \"general.remove\" | resolve }}\' class=\'icon-only btn-icon btn\' data-ng-disabled=\'!$ctrl.removable\' data-ng-click=\'$ctrl.removeConversion()\'>\r\n                <i aria-hidden=\'true\' class=\'icon-bin\'></i>\r\n            </button>\r\n        </div>\r\n        <cms-edit-conversion is-funnel=\'$ctrl.isFunnel\' conversion=\'$ctrl.conversion\' enabled=\'$ctrl.editable\' on-updated=\'$ctrl.updateConversion\'>\r\n        </cms-edit-conversion>\r\n    </div>\r\n</div>");
$templateCache.put("cms.webanalytics/campaign/reportSetup/conversions/conversion/editConversion.component.html","<div>\r\n    <button title=\'{{ \"general.edit\" | resolve }}\' class=\'icon-only btn-icon btn\' data-ng-disabled=\'!$ctrl.enabled\' data-ng-click=\'$ctrl.openDialog()\'>\r\n        <i aria-hidden=\'true\' class=\'icon-edit\'></i>\r\n    </button>\r\n</div>");
$templateCache.put("cms.webanalytics/campaign/reportSetup/objective/dialog/modifyObjective.dialog.html","<div id=\'js-page-selector-container\'>\r\n    <cms-dialog-header on-close=\'$ctrl.dismiss\' header=\'{{ $ctrl.title }}\'></cms-dialog-header>\r\n    <div class=\'dialog-content\'>\r\n        <div class=\'campaign-edit-dialog\'>\r\n            <form class=\'form-horizontal\' name=\'$ctrl.form\'>\r\n                <cms-select value=\'$ctrl.objective.conversionID\'\r\n                            options=\'$ctrl.objectiveConversions\'\r\n                            title=\'{{ \"campaign.objective.conversion\" | resolve }}\'\r\n                            label=\'{{ \"campaign.objective.conversion\" | resolve }}\'\r\n                            required-error-text=\'{{ \"campaign.objective.conversion.errorText\" | resolve }}\'\r\n                            id=\'selectConversion\'\r\n                            required=\'true\'>\r\n                </cms-select>                \r\n\r\n                <cms-textbox value=\'$ctrl.objective.value\' maxlength=\'9\'\r\n                             title=\'{{ \"campaign.objective.target\" | resolve }}\'\r\n                             label=\'{{ \"campaign.objective.target\" | resolve }}\'\r\n                             required=\'true\'\r\n                             pattern=\'$ctrl.targetRegexPattern\'\r\n                             pattern-error=\'{{ \"campaign.objective.target.errorText\" | resolve }}\'\r\n                             explanation-text=\'{{ \"campaign.objective.target.explanationText\" | resolve }}\'\r\n                             id=\'targetValue\'>\r\n                </cms-textbox>\r\n            </form>\r\n        </div>\r\n    </div>\r\n    <cms-dialog-footer confirm-label=\'{{ \"general.save\" | resolve }}\' on-confirm=\'$ctrl.confirm\'\r\n                       dismiss-label=\'{{ \"general.cancel\" | resolve }}\' on-dismiss=\'$ctrl.dismiss\'>        \r\n    </cms-dialog-footer>\r\n</div>");}]);return 'cms.webanalytics/campaign/';}})