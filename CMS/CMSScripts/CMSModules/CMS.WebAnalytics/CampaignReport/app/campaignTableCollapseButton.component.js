(function (angular) {
	'use strict';

	angular.module('cms.webanalytics/campaignreport/campaignTableCollapseButton.component', [])
        .component('cmsCollapseButton', button());

	function button() {
		return {
			bindings: {
			    collapsed: '='
			},
			templateUrl: 'cms.webanalytics/campaignreport/campaignTableCollapseButton.component.html'
		};
	}

}(angular));