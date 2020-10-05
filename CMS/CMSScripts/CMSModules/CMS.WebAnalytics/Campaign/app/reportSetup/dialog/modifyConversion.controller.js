(function (angular, _) {
    'use strict';

    angular.module('cms.webanalytics/campaign/reportSetup/modifyConversion.controller', [
        'cms.webanalytics/campaign/reportSetup/dialog/selectParameter.directive',
        'cms.webanalytics/campaign/cms.application.service'
    ])
        .controller('ModifyConversionController', controller);

    /*@ngInject*/
    function controller($scope, $uibModalInstance, $interpolate, conversion, title, activityTypes, resolveFilter, applicationService, serverDataService) {
        var ctrl = this,
            application = applicationService.application,
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
            return selectedActivityType === 'pagevisit';
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
                areParametersRequired: false,
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