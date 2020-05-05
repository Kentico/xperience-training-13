(function (angular, application, dataFromServer) {
    'use strict';
    
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