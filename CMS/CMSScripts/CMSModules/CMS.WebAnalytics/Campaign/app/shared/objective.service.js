(function (angular) {
    'use strict';

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