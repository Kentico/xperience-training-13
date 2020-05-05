cmsdefine([], function () {

    /**
     * Represents one campaign detail in campaign list.
     */
    var Controller = function ($scope, $filter, resolveFilter) {
        var DELETE_CONFIRMATION_RESOURCE_STRING = 'general.deleteconfirmation',
            // The number of milliseconds in one day
            ONE_DAY = 1000 * 60 * 60 * 24,
            campaign = $scope.campaign;

        this.$scope = $scope;
        this.$scope.model = {
            /**
             * Returns the time portion of the given date.
             */
            extractTime: function (date) {
                return $filter('date')(date, 'shortTime');
            },

            /**
             * Emits event that campaign wants to be deleted.
             */
            deleteCampaignClick: function () {
                if (confirm(resolveFilter(DELETE_CONFIRMATION_RESOURCE_STRING))) {
                    $scope.$emit('deleteCampaign', campaign);
                }
            },
        };
    },
        directive = function () {
            return {
                restrict: 'A',
                templateUrl: 'CampaignDetailTemplate.html',
                controller: Controller,
                scope: {
                    campaign: '='
                }
            };
        };

    Controller.$inject = [
      '$scope',
      '$filter',
      'resolveFilter'
    ];

    return [directive];
});