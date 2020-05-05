cmsdefine(['Underscore'], function (_) {
    return function () {
        return ['$scope', 'cmsCampaignResource', 'serverData', function ($scope, campaignResource, serverData) {
            var formatCampaignObjective = function (campaign) {
                // Objective can be null (not set), 0 (set but no progress yet) or >0
                // Simplified if statement cannot be used -> zero objective wouldn't be visible.
                if (campaign.objective != null) {
                    campaign.objective = Math.round(campaign.objective * 10) / 10;
                }
                return campaign;
            };

            $scope.model = {
                campaigns: serverData.campaigns.map(formatCampaignObjective),
                newCampaignClick: function () {
                    top.location = serverData.newCampaignLink;
                }
            };

            $scope.$watch("statusFilter", function(newValue) {
                if ((newValue !== "") && ($scope.sortBy === "")) {
                    $scope.sortBy = "displayName";
                }
            });

            $scope.$on('deleteCampaign', function (event, campaign) {
                campaignResource.delete({ id: campaign.campaignID });

                $scope.model.campaigns = _.reject($scope.model.campaigns, function (item) {
                    return item.campaignID === campaign.campaignID;
                });
            });
        }];
    };
});