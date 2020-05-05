(function (angular) {
    'use strict';

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