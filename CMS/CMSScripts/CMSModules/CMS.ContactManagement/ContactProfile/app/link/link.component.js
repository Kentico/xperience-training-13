(function (angular) {
	'use strict';

	angular
        .module('cms.contactmanagement/contactprofile/link.component', ['ngSanitize'])
		.component('cmsContactLink', link());

	function link() {
		var component = {
			bindings: {
				contactUrl: '<'
			},
			templateUrl: 'cms.contactmanagement/contactprofile/link/link.component.html',
            controller: controller
		};

		return component;
	}

    /*@ngInject*/
	function controller() {
		this.openDialog = function () {
			if (this.contactUrl) {
				modalDialog(this.contactUrl, 'ContactDetail', '95%', '95%');
			}
		};
	}

}(angular));