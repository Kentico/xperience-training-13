(function (angular) {
	'use strict';

	angular
        .module('cms.contactmanagement/contactprofile/notes.component', ['ngSanitize'])
        .component('cmsContactNotes', notes());

	function notes() {
		var component = {
			bindings: {
			    notes: '<'
			},
			templateUrl: 'cms.contactmanagement/contactprofile/notes/notes.component.html',
            controller: controller
		};

		return component;
	}


    /*@ngInject*/
	function controller($sce) {
	    this.notes = $sce.trustAsHtml(this.notes);
	}

}(angular));