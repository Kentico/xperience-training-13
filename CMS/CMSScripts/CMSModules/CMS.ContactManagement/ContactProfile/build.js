cmsdefine(['angular','CMS/Filters/Resolve','angular-resource','angular-sanitize','CMS/UrlHelper','CMS/Application'], function(angular,resolve,ngResource,ngSanitize,urlHelper,application) { 
var moduleName = 'cms.contactmanagement/contactprofile/';
return function(dataFromServer) { 
if(angular && resolve && dataFromServer && dataFromServer.resources) { 
resolve(angular, dataFromServer.resources); 
} 
(function(angular, moduleName, urlHelper) {
    'use strict';
    controller.$inject = ["$location"];
    angular.module(moduleName, [
            moduleName + "app.templates",
            'cms.contactmanagement/contactProfile.component',
            'cms.contactmanagement/contactprofile/card.component.wrapper'
        ])
        .controller('app', controller);
    

    function controller($location) {
        this.contactId = urlHelper.getParameters($location.absUrl()).contactid;
    }

})(angular, moduleName, urlHelper);


(function (angular) {
    'use strict';

    controller.$inject = ["contactProfileService"];
    angular.module('cms.contactmanagement/contactProfile.component', [
        'cms.contactmanagement/contactProfileSimple.component',
        'cms.contactmanagement/contactProfileFull.component',
        'cms.contactmanagement/contactProfile.service',
        'CMS/Filters.Resolve'
    ])
    .component('cmsContactProfile', contact());


    function contact() {
        var component = {
            bindings: {
                contactId: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/contactProfile.component.html',
            controller: controller
        };

        return component;
    };


    /*@ngInject*/
    function controller(contactProfileService) {
        activate.apply(this);

        function activate() {
            contactProfileService.getContact(this.contactId)
                .then(onSuccess.bind(this))
                .catch(onError.bind(this));
        };

        function onSuccess(contact) {
            this.contact = contact;
            if (contact.ContactType === "Simple") {
                this.simpleContact = true;
            } else {
                this.fullContact = true;
            }
        };

        function onError() {
            this.error = true;
        };
    };
}(angular));
(function(angular) {
    'use strict';

    contactProfileResourceFactory.$inject = ["$resource", "applicationService"];
    contactProfileService.$inject = ["contactProfileResourceFactory"];
    angular.module('cms.contactmanagement/contactProfile.service', [
            'cms.contactmanagement/contactprofile/cms.application.service',
            'ngResource'
    ])
    .factory('contactProfileResourceFactory', contactProfileResourceFactory)
    .service('contactProfileService', contactProfileService);


    /*@ngInject*/
    function contactProfileResourceFactory($resource, applicationService) {
        return $resource(applicationService.application.getData('applicationUrl') + 'cmsapi/contactprofile?contactId=:contactId');
    };
    
    
    /*@ngInject*/
    function contactProfileService(contactProfileResourceFactory) {
        this.getContact = function (contactId) {
            return contactProfileResourceFactory.get({ contactId: contactId }).$promise;
        }
    };

}(angular));
(function (angular, dataFromServer) {
    'use strict';

    angular.module('cms.contactmanagement/contactProfileFull.component', [
        'cms.contactmanagement/contactprofile/card.component',
        'cms.contactmanagement/contactprofile/categoryHeading.component',
        'cms.contactmanagement/contactprofile/contactGroupsMembership.component',
        'cms.contactmanagement/contactprofile/newsletterSubscriptions.component',
        'cms.contactmanagement/contactprofile/persona.component',
        'cms.contactmanagement/contactprofile/scorings.component',
        'cms.contactmanagement/contactprofile/submittedForms.component',
        'cms.contactmanagement/contactprofile/notes.component',
        'cms.contactmanagement/contactprofile/detail.component',
        'cms.contactmanagement/contactprofile/journey.component',
        'CMS/Filters.Resolve'
    ])
    .component('cmsContactProfileFull', contact());


    function contact() {
        var component = {
            bindings: {
                contact: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/contactProfileFull.component.html',
            controller: controller
        };

        return component;
    };

    function controller() {
        this.personaModuleAvailable = dataFromServer.personaModuleAvailable;
        this.formModuleAvailable = dataFromServer.formModuleAvailable;
        this.newsletterModuleAvailable = dataFromServer.newsletterModuleAvailable;
        this.activitiesExist = dataFromServer.activitiesExist;
    };

}(angular, dataFromServer));
(function (angular, dataFromServer) {
    'use strict';

    angular.module('cms.contactmanagement/contactProfileSimple.component', [
        'cms.contactmanagement/contactprofile/card.component',
        'cms.contactmanagement/contactprofile/categoryHeading.component',
        'cms.contactmanagement/contactprofile/contactGroupsMembership.component',
        'cms.contactmanagement/contactprofile/newsletterSubscriptions.component',
        'cms.contactmanagement/contactprofile/notes.component',
        'CMS/Filters.Resolve'
    ])
    .component('cmsContactProfileSimple', contact());


    function contact() {
        var component = {
            bindings: {
                contact: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/contactProfileSimple.component.html',
            controller: controller
        };

        return component;
    };

    function controller() {
        this.newsletterModuleAvailable = dataFromServer.newsletterModuleAvailable;
    };

}(angular, dataFromServer));
(function (angular) {
    'use strict';

    angular.module('cms.contactmanagement/contactprofile/card.component', [
        'cms.contactmanagement/contactprofile/card/marketingEmailStatus.component',
        'cms.contactmanagement/contactprofile/card/address.component',
        'CMS/Filters.Resolve'
    ])
        .component('cmsCard', card());


    function card() {
        var component = {
            bindings: {
                contact: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/card/card.component.html',
            controller: controller
        };

        return component;
    }


    /*@ngInject*/
    function controller() {
        this.contactCard = this.contact;

        this.showContactAddress = false;
        this.showContactAddress = this.contact.ContactEmail || this.contact.ContactAddress ||
            (this.contact.ContactName && (this.contact.ContactName.indexOf('Anonymous') !== 0));
    };

}(angular));
(function (angular) {
    'use strict';

    controller.$inject = ["contactProfileService"];
    angular.module('cms.contactmanagement/contactprofile/card.component.wrapper', [
        'cms.contactmanagement/contactprofile/card.component',
        'cms.contactmanagement/contactProfile.service',
        'CMS/Filters.Resolve'
    ])
        .component('cmsCardWrapper', card());


    function card() {
        var component = {
            bindings: {
                contactId: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/card/card.component.wrapper.html',
            controller: controller
        };

        return component;
    }


    /*@ngInject*/
    function controller(contactProfileService) {
        activate.apply(this);

        function activate() {
            contactProfileService.getContact(this.contactId)
                .then(onSuccess.bind(this))
                .catch(onError.bind(this));
        }

        function onSuccess(contact) {
            this.contact = contact;
        }

        function onError() {
            this.error = true;
        }
    }

}(angular));
(function(angular) {
    'use strict';

    controller.$inject = ["contactGroupsMembershipService"];
    angular
        .module('cms.contactmanagement/contactprofile/contactGroupsMembership.component', [
            'cms.contactmanagement/contactprofile/contactGroupsMembership.service'
        ])
        .component('cmsContactGroupsMembership', contactGroupsMembership());


    function contactGroupsMembership() {
        var component = {
            bindings: {
                contactId: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/contactGroupsMembership/contactGroupsMembership.component.html',
            controller: controller
        };

        return component;
    };


    /*@ngInject*/
    function controller(contactGroupsMembershipService) {
        activate.apply(this);

        function activate() {
            contactGroupsMembershipService.getMembershipsForContact(this.contactId)
                .then(onSuccess.bind(this));
        };
                
        function onSuccess(memberships) {
            this.memberships = memberships;
        }
    };

}(angular));
(function(angular) {
    'use strict';

    contactGroupsMembershipResourceFactory.$inject = ["$resource", "applicationService"];
    contactGroupsMembershipService.$inject = ["contactGroupsMembershipResourceFactory"];
    angular
        .module('cms.contactmanagement/contactprofile/contactGroupsMembership.service', [
            'cms.contactmanagement/contactprofile/cms.application.service',
            'ngResource'
        ])
        .factory('contactGroupsMembershipResourceFactory', contactGroupsMembershipResourceFactory)
        .service('contactGroupsMembershipService', contactGroupsMembershipService);
    

    /*@ngInject*/
    function contactGroupsMembershipResourceFactory($resource, applicationService) {
        return $resource(applicationService.application.getData('applicationUrl') + 'cmsapi/contactGroupsMembership?contactId=:contactId');
    };


    /*@ngInject*/
    function contactGroupsMembershipService(contactGroupsMembershipResourceFactory) {
        this.getMembershipsForContact = function(contactId) {
            return contactGroupsMembershipResourceFactory.query({ contactId: contactId }).$promise;
        }
    };

}(angular));
(function (angular) {
    'use strict';

    controller.$inject = ["detailService"];
    angular.module('cms.contactmanagement/contactprofile/detail.component', [
        'cms.contactmanagement/contactprofile/detail.service',
        'cms.contactmanagement/contactprofile/detail/accounts.component',
        'cms.contactmanagement/contactprofile/detail/campaign/campaign.component',
        'CMS/Filters.Resolve'
    ])
    .component('cmsDetail', details());


    function details() {
        var component = {
            bindings: {
                contactId: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/detail/detail.component.html',
            controller: controller
        };

        return component;
    };


    /*@ngInject*/
    function controller(detailService) {
        function activate() {
            detailService.getDetail(this.contactId)
                .then(onSuccess.bind(this));
        };

        function onSuccess(fields) {
            this.fields = fields;
        };

        activate.apply(this);
    };
}(angular));
(function (angular) {
    'use strict';

    detailResourceFactory.$inject = ["$resource", "applicationService"];
    detailService.$inject = ["detailResourceFactory"];
    angular
        .module('cms.contactmanagement/contactprofile/detail.service', [
            'cms.contactmanagement/contactprofile/cms.application.service',
            'ngResource'
        ])
        .factory('detailResourceFactory', detailResourceFactory)
        .service('detailService', detailService);


    /*@ngInject*/
    function detailResourceFactory($resource, applicationService) {
        return $resource(applicationService.application.getData('applicationUrl') + 'cmsapi/ContactDetails?contactId=:contactId');
    };


    /*@ngInject*/
    function detailService(detailResourceFactory) {
        this.getDetail = function (contactId) {
            return detailResourceFactory.query({ contactId: contactId }).$promise;
        };
    };
}(angular));
(function (angular) {
    'use strict';

    controller.$inject = ["journeyService"];
    angular.module('cms.contactmanagement/contactprofile/journey.component', [
        'cms.contactmanagement/contactprofile/journey.service'
    ])
    .component('cmsContactJourney', journey());


    function journey() {
        var component = {
            bindings: {
                contactId: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/journey/journey.component.html',
            controller: controller
        };

        return component;
    };


    /*@ngInject*/
    function controller(journeyService) {
        function activate() {
            journeyService.getJourneyForContact(this.contactId).then(onSuccess.bind(this));
        };

        function onSuccess(journey) {
            this.journey = journey;
        };

        activate.apply(this);
    };
}(angular));
(function (angular) {
    'use strict';

    journeyResourceFactory.$inject = ["$resource", "applicationService"];
    journeyService.$inject = ["journeyResourceFactory"];
    angular
        .module('cms.contactmanagement/contactprofile/journey.service', [
            'cms.contactmanagement/contactprofile/cms.application.service',
            'ngResource'
        ])
        .factory('journeyResourceFactory', journeyResourceFactory)
        .service('journeyService', journeyService);


    /*@ngInject*/
    function journeyResourceFactory($resource, applicationService) {
        return $resource(applicationService.application.getData('applicationUrl') + 'cmsapi/contactJourney?contactId=:contactId');
    };


    /*@ngInject*/
    function journeyService(journeyResourceFactory) {
        this.getJourneyForContact = function (contactId) {
            return journeyResourceFactory.get({ contactId: contactId }).$promise;
        }
    };

}(angular));
(function (angular) {
    'use strict';

    controller.$inject = ["newsletterSubscriptionsService"];
    angular
        .module('cms.contactmanagement/contactprofile/newsletterSubscriptions.component', [
            'cms.contactmanagement/contactprofile/newsletterSubscriptions.service'
        ])
        .component('cmsNewsletterSubscriptions', newsletterSubscriptions());


    function newsletterSubscriptions() {
        var component = {
            bindings: {
                contactId: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/newsletterSubscriptions/newsletterSubscriptions.component.html',
            controller: controller
        };

        return component;
    };


    /*@ngInject*/
    function controller(newsletterSubscriptionsService) {
        activate.apply(this);

        function activate() {
            newsletterSubscriptionsService.getSubscriptionsForContact(this.contactId)
                .then(onSuccess.bind(this));
        };

        function onSuccess(subscriptions) {
            this.subscriptions = subscriptions;
        }
    };

}(angular));
(function (angular) {
    'use strict';

    newsletterSubscriptionsResourceFactory.$inject = ["$resource", "applicationService"];
    newsletterSubscriptionsService.$inject = ["newsletterSubscriptionsResourceFactory"];
    angular
        .module('cms.contactmanagement/contactprofile/newsletterSubscriptions.service', [
            'cms.contactmanagement/contactprofile/cms.application.service',
            'ngResource'
        ])
        .factory('newsletterSubscriptionsResourceFactory', newsletterSubscriptionsResourceFactory)
        .service('newsletterSubscriptionsService', newsletterSubscriptionsService);


    /*@ngInject*/    
    function newsletterSubscriptionsResourceFactory($resource, applicationService) {
        return $resource(applicationService.application.getData('applicationUrl') + 'cmsapi/contactNewsletterSubscriptions?contactId=:contactId');
    };


    /*@ngInject*/
    function newsletterSubscriptionsService(newsletterSubscriptionsResourceFactory) {
        this.getSubscriptionsForContact = function (contactId) {
            return newsletterSubscriptionsResourceFactory.query({ contactId: contactId }).$promise;
        }
    };

}(angular));
(function (angular) {
	'use strict';

	controller.$inject = ["$sce"];
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
(function (angular) {
    'use strict';

    controller.$inject = ["personaService"];
    angular
        .module('cms.contactmanagement/contactprofile/persona.component', ['cms.contactmanagement/contactprofile/persona.service'])
        .component('cmsPersona', persona());


    function persona() {
        var component = {
            bindings: {
                contactId: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/persona/persona.component.html',
            controller: controller
        };

        return component;
    }


    /*@ngInject*/
    function controller(personaService) {
        activate.apply(this);

        function activate() {
            personaService.getPersona(this.contactId)
                .then(onSuccess.bind(this));
        };

        function onSuccess(persona) {
            this.persona = persona;
        }
    }

}(angular));
(function (angular) {
    'use strict';

    personaResourceFactory.$inject = ["$resource", "applicationService"];
    personaService.$inject = ["personaResourceFactory"];
    angular
        .module('cms.contactmanagement/contactprofile/persona.service', [
            'cms.contactmanagement/contactprofile/cms.application.service',
            'ngResource'
        ])
        .factory('personaResourceFactory', personaResourceFactory)
        .service('personaService', personaService);


    /*@ngInject*/
    function personaResourceFactory($resource, applicationService) {
        return $resource(applicationService.application.getData('applicationUrl') + 'cmsapi/ContactPersona?contactId=:contactId');
    };


    /*@ngInject*/
    function personaService(personaResourceFactory) {
        this.getPersona = function (contactId) {
            return personaResourceFactory.get({ contactId: contactId }).$promise;
        }
    };

}(angular));
(function (angular) {
    'use strict';

    controller.$inject = ["scoringService"];
    angular
        .module('cms.contactmanagement/contactprofile/scorings.component', ['cms.contactmanagement/contactprofile/scorings.service'])
        .component('cmsScorings', scorings());


    function scorings() {
        var component = {
            bindings: {
                contactId: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/scorings/scorings.component.html',
            controller: controller
        };

        return component;
    }


    /*@ngInject*/
    function controller(scoringService) {
        activate.apply(this);

        function activate() {
            scoringService.getScorings(this.contactId)
                .then(onSuccess.bind(this));
        };

        function onSuccess(scorings) {
            this.scorings = scorings;
        }
    }
}(angular));
(function (angular) {
    'use strict';

    scoringResourceFactory.$inject = ["$resource", "applicationService"];
    scoringService.$inject = ["scoringResourceFactory"];
    angular
        .module('cms.contactmanagement/contactprofile/scorings.service', [
            'cms.contactmanagement/contactprofile/cms.application.service',
            'ngResource'
        ])
        .factory('scoringResourceFactory', scoringResourceFactory)
        .service('scoringService', scoringService);


    /*@ngInject*/
    function scoringResourceFactory($resource, applicationService) {
        return $resource(applicationService.application.getData('applicationUrl') + 'cmsapi/ContactScoring?contactId=:contactId');
    };


    /*@ngInject*/
    function scoringService(scoringResourceFactory) {
        this.getScorings = function(contactId) {
            return scoringResourceFactory.query({ contactId: contactId }).$promise;
        };
    };
}(angular));
(function(angular) {
    'use strict';

    angular
        .module('cms.contactmanagement/contactprofile/categoryHeading.component', [])
        .component('cmsCategoryHeading', categoryHeading());

    function categoryHeading() {
        var component = {
            bindings: {
                heading: '@'
            },
            transclude: true,
            templateUrl: 'cms.contactmanagement/contactprofile/shared/categoryHeading.component.html'
        };

        return component;
    };

}(angular));
/**
 * Wraps the CMS.Application module into Angular service to leverage Angular DI system and simplify the tests.
 */
(function (angular, application) {
	'use strict';

    angular.module('cms.contactmanagement/contactprofile/cms.application.service', [])
        .service('applicationService', applicationService);


    /*@ngInject*/    
    function applicationService() {
        this.application = application;
    }

}(angular, application));
(function (angular) {
    'use strict';

    controller.$inject = ["submittedFormsService"];
    angular
        .module('cms.contactmanagement/contactprofile/submittedForms.component', [
            'cms.contactmanagement/contactprofile/submittedForms.service',
            'CMS/Filters.Resolve'
        ])
        .component('cmsSubmittedForms', submittedForms());


    function submittedForms() {
        var component = {
            bindings: {
                contactId: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/submittedForms/submittedForms.component.html',
            controller: controller
        };

        return component;
    };


    /*@ngInject*/
    function controller(submittedFormsService) {
        var self = this;
        activate();

        self.sortTypeName = 'FormDisplayName';
        self.sortTypeSite = 'SiteDisplayName';
        self.sortTypeDate = 'FormSubmissionDate';

        self.sortType = self.sortTypeDate;
        self.sortAsc = true;

        self.sort = function (type) {
            if (self.sortType === type) {
                self.sortAsc = !self.sortAsc;
            } else {
                self.sortType = type;
                switch (type) {
                    case self.sortTypeName:
                    case self.sortTypeSite:
                        self.sortAsc = false;
                        break;
                    case self.sortTypeDate:
                        self.sortAsc = true;
                        break;
                }
            }
        };

        self.showSorting = function(type, asc) {
            return self.sortType === type && asc;
        };

        function activate() {
            submittedFormsService.getSubmittedForms(self.contactId)
                .then(onSuccess);
        };

        function onSuccess(forms) {
            self.forms = forms;
        };
    };
}(angular));
(function (angular) {
    'use strict';

    submittedFormsResourceFactory.$inject = ["$resource", "applicationService"];
    submittedFormsService.$inject = ["submittedFormsResourceFactory"];
    angular
        .module('cms.contactmanagement/contactprofile/submittedForms.service', [
            'cms.contactmanagement/contactprofile/cms.application.service',
            'ngResource'
        ])
        .factory('submittedFormsResourceFactory', submittedFormsResourceFactory)
        .service('submittedFormsService', submittedFormsService);


    /*@ngInject*/
    function submittedFormsResourceFactory($resource, applicationService) {
        return $resource(applicationService.application.getData('applicationUrl') + 'cmsapi/contactSubmittedForms?contactId=:contactId');
    };


    /*@ngInject*/
    function submittedFormsService(submittedFormsResourceFactory) {
        this.getSubmittedForms = function (contactId) {
            return submittedFormsResourceFactory.query({ contactId: contactId }).$promise;
        }
    };

}(angular));
(function (angular, dataFromServer) {
    'use strict';
    
    angular.module('cms.contactmanagement/contactprofile/card/address.component', [])
        .component('cmsCardAddress', cardAddress());

    function cardAddress() {
        var component = {
            bindings: {
                contactCard: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/card/address/address.component.html',
            controller: controller
        }
        return component;
    }

    /*@ngInject*/
    function controller() {
        this.isMale = this.contactCard.ContactGender === 'Male';
        this.isFemale = this.contactCard.ContactGender === 'Female';
        this.showTags = this.contactCard.IsCustomer || this.contactCard.IsUser;
        this.showMarketingEmailStatus = dataFromServer.newsletterModuleAvailable;
    }
}(angular, dataFromServer));
(function (angular) {
    'use strict';

    controller.$inject = ["marketingEmailStatusService"];
    angular
        .module('cms.contactmanagement/contactprofile/card/marketingEmailStatus.component', [
            'cms.contactmanagement/contactprofile/card/marketingEmailStatus.service',
            'CMS/Filters.Resolve'
        ])
        .component('cmsCardMarketingEmailStatus', cardMarketingEmail());
    

    function cardMarketingEmail() {
        var component = {
            bindings: {
                contactId: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/card/marketingEmailStatus/marketingEmailStatus.component.html',
            controller: controller
        };

        return component;
    };


    /*@ngInject*/
    function controller(marketingEmailStatusService) {
        activate.apply(this);

        function activate() {
            marketingEmailStatusService.getMarketingEmailStatus(this.contactId)
                .then(onSuccess.bind(this));
        };

        function onSuccess(marketingEmailStatusViewModel) {
            this.marketingEmailStatus = marketingEmailStatusViewModel.MarketingEmailStatus;
        };
    };
}(angular));
(function (angular) {
    'use strict';

    marketingEmailStatusResourceFactory.$inject = ["$resource", "applicationService"];
    marketingEmailStatusService.$inject = ["marketingEmailStatusResourceFactory"];
    angular
        .module('cms.contactmanagement/contactprofile/card/marketingEmailStatus.service', [
            'cms.contactmanagement/contactprofile/cms.application.service',
            'ngResource'
        ])
        .factory('marketingEmailStatusResourceFactory', marketingEmailStatusResourceFactory)
        .service('marketingEmailStatusService', marketingEmailStatusService);


    /*@ngInject*/
    function marketingEmailStatusResourceFactory($resource, applicationService) {
        return $resource(applicationService.application.getData('applicationUrl') + 'cmsapi/contactMarketingEmailStatus?contactId=:contactId');
    };


    /*@ngInject*/
    function marketingEmailStatusService(marketingEmailStatusResourceFactory) {
        this.getMarketingEmailStatus = function (contactId) {
            return marketingEmailStatusResourceFactory.get({ contactId: contactId }).$promise;
        }
    };

}(angular));
(function (angular) {
    'use strict';

    angular.module('cms.contactmanagement/contactprofile/detail/accounts.component', [
        'cms.contactmanagement/contactprofile/detail/accounts/account.component',
        'CMS/Filters.Resolve'
    ])
    .component('cmsAccounts', accountList());

    function accountList() {
        var component = {
            bindings: {
                field: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/detail/accounts/accounts.component.html',
        };

        return component;
    };
}(angular));
(function (angular) {
    'use strict';

    angular.module('cms.contactmanagement/contactprofile/detail/campaign/campaign.component', [])
        .component('cmsCampaign', campaign());

    function campaign() {
        var component = {
            bindings: {
                field: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/detail/campaign/campaign.component.html',
        };

        return component;
    };
}(angular));
(function (angular) {
    'use strict';

    angular.module('cms.contactmanagement/contactprofile/detail/accounts/account.component', [
        'CMS/Filters.Resolve'
    ])
    .component('cmsAccount', account());

    function account() {
        var component = {
            bindings: {
                account: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/detail/accounts/account/account.component.html',
        };

        return component;
    };
}(angular));
angular.module("cms.contactmanagement/contactprofile/app.templates", []).run(["$templateCache", function($templateCache) {$templateCache.put("cms.contactmanagement/contactprofile/contactProfile.component.html","<span data-ng-cloak=\'\' data-ng-if=\'$ctrl.error\'>\r\n    {{ \"editedobject.notexists\" | resolve }}\r\n</span>\r\n<div data-ng-cloak=\'\' data-ng-if=\'$ctrl.simpleContact\'>\r\n    <cms-contact-profile-simple contact=\'$ctrl.contact\'></cms-contact-profile-simple>\r\n</div>\r\n<div data-ng-cloak=\'\' data-ng-if=\'$ctrl.fullContact\'>\r\n    <cms-contact-profile-full contact=\'$ctrl.contact\'></cms-contact-profile-full>\r\n</div>");
$templateCache.put("cms.contactmanagement/contactprofile/contactProfileFull.component.html","<cms-card contact=\'$ctrl.contact\'></cms-card>\r\n\r\n<cms-category-heading heading=\'{{ \"contactmanagement.contact.contactinfo\" | resolve }}\'>\r\n    <div class=\'row\'>\r\n        <div class=\'col-md-4 cms-details\'>\r\n            <cms-detail class=\'component\' contact-id=\'$ctrl.contact.ContactID\'></cms-detail>\r\n        </div>\r\n        <div class=\'col-md-8\'>\r\n            <cms-persona class=\'component\' contact-id=\'$ctrl.contact.ContactID\' data-ng-if=\'$ctrl.personaModuleAvailable\'></cms-persona>\r\n            <cms-scorings class=\'component\' contact-id=\'$ctrl.contact.ContactID\'></cms-scorings>\r\n        </div>\r\n    </div>\r\n</cms-category-heading>\r\n\r\n<cms-category-heading heading=\'{{ \"om.contact.journey.header\" | resolve }}\' data-ng-if=\'$ctrl.activitiesExist\'>\r\n    <cms-contact-journey class=\'component cms-journey\' contact-id=\'$ctrl.contact.ContactID\'></cms-contact-journey>\r\n</cms-category-heading>\r\n\r\n\r\n<cms-category-heading heading=\'{{ \"contactmanagement.contact.newslettersubscriptions\" | resolve }}\' data-ng-if=\'$ctrl.newsletterModuleAvailable\'>\r\n    <cms-newsletter-subscriptions class=\'component\' contact-id=\'$ctrl.contact.ContactID\'></cms-newsletter-subscriptions>\r\n</cms-category-heading>\r\n\r\n\r\n<cms-category-heading heading=\'{{ \"contactmanagement.contact.contactgroupmemberships\" | resolve }}\'>\r\n    <cms-contact-groups-membership class=\'component\' contact-id=\'$ctrl.contact.ContactID\'></cms-contact-groups-membership>\r\n</cms-category-heading>\r\n\r\n\r\n<cms-category-heading heading=\'{{ \"contactmanagement.contact.submittedforms\" | resolve }}\' data-ng-if=\'$ctrl.formModuleAvailable\'>\r\n    <cms-submitted-forms class=\'component\' contact-id=\'$ctrl.contact.ContactID\'></cms-submitted-forms>\r\n</cms-category-heading>\r\n\r\n<cms-category-heading heading=\'{{ \"contactmanagement.contact.notes\" | resolve }}\'>\r\n    <cms-contact-notes class=\'component\' notes=\'$ctrl.contact.ContactNotes\'></cms-contact-notes>\r\n</cms-category-heading>");
$templateCache.put("cms.contactmanagement/contactprofile/contactProfileSimple.component.html","<cms-card contact=\'$ctrl.contact\'></cms-card>\r\n\r\n<cms-category-heading heading=\'{{ \"contactmanagement.contact.newslettersubscriptions\" | resolve }}\' data-ng-if=\'$ctrl.newsletterModuleAvailable\'>\r\n    <cms-newsletter-subscriptions class=\'component\' contact-id=\'$ctrl.contact.ContactID\'></cms-newsletter-subscriptions>\r\n</cms-category-heading>\r\n\r\n<cms-category-heading heading=\'{{ \"contactmanagement.contact.contactgroupmemberships\" | resolve }}\'>\r\n    <cms-contact-groups-membership class=\'component\' contact-id=\'$ctrl.contact.ContactID\'></cms-contact-groups-membership>\r\n</cms-category-heading>");
$templateCache.put("cms.contactmanagement/contactprofile/card/card.component.html","<div class=\'header-actions-container\'>\r\n    <a class=\'btn btn-primary\' data-ng-href=\'{{::$ctrl.contact.EditUrl}}\'>{{ \"contactmanagement.contact.editcontact\" | resolve }}</a>\r\n</div>\r\n\r\n<div class=\'head-section\'>\r\n    <h3 data-ng-bind=\'::$ctrl.contactCard.ContactName\'></h3>\r\n    <cms-card-address contact-card=\'::$ctrl.contactCard\' data-ng-if=\'::$ctrl.showContactAddress\'></cms-card-address>\r\n</div>");
$templateCache.put("cms.contactmanagement/contactprofile/card/card.component.wrapper.html","<div class=\'cms-contact-profile\'>\r\n    <cms-card contact=\'$ctrl.contact\' data-ng-if=\'$ctrl.contact\'></cms-card>\r\n</div>");
$templateCache.put("cms.contactmanagement/contactprofile/contactGroupsMembership/contactGroupsMembership.component.html","<div>\r\n    <ul>\r\n        <li data-ng-repeat=\'contactGroup in $ctrl.memberships track by $index\'>\r\n            <a target=\'_blank\' data-ng-href=\'{{::contactGroup.ContactGroupUrl}}\'>{{::contactGroup.ContactGroupDisplayName}}</a>\r\n        </li>\r\n    </ul>\r\n    <span data-ng-show=\'$ctrl.memberships && !$ctrl.memberships.length\'>{{ \"contactmanagement.contact.contactgroupmemberships.nodata\" | resolve }}</span>\r\n</div>");
$templateCache.put("cms.contactmanagement/contactprofile/detail/detail.component.html","<h4>{{ \"contactmanagement.contact.detail\" | resolve }}</h4>\r\n<p data-ng-if=\'$ctrl.fields && $ctrl.fields.length === 0\'>{{ \"contactmanagement.contact.detail.empty\" | resolve }}</p>\r\n\r\n<ul class=\'field-list\' data-ng-if=\'$ctrl.fields && $ctrl.fields.length > 0\'>\r\n    <li on=\'field.FieldType\' class=\'row\' data-ng-repeat=\'field in $ctrl.fields\' data-ng-switch=\'\'>\r\n        <span class=\'field-name col-xs-3 col-md-4\'>{{::field.FieldCaption}}</span>\r\n        \r\n        <cms-accounts field=\'field\' class=\'col-xs-9 col-md-8\' data-ng-switch-when=\'List`1\'></cms-accounts>\r\n        <cms-campaign field=\'field.FieldValue\' class=\'col-xs-9 col-md-8\' data-ng-switch-when=\'ContactDetailsCampaignViewModel\'></cms-campaign>\r\n\r\n        <span class=\'col-xs-9 col-md-8 field-value\' data-ng-switch-when=\'DateTime\'>{{::field.FieldValue | date:\'MMM d, yyyy\'}}</span>\r\n        <span class=\'col-xs-9 col-md-8 field-value\' data-ng-switch-default=\'\'>{{::field.FieldValue}}</span>\r\n    </li>\r\n</ul>");
$templateCache.put("cms.contactmanagement/contactprofile/newsletterSubscriptions/newsletterSubscriptions.component.html","<div>\r\n    <ul>\r\n        <li data-ng-repeat=\'newsletterSubscription in $ctrl.subscriptions track by $index\'>\r\n            <a target=\'_blank\' data-ng-href=\'{{::newsletterSubscription.NewsletterUrl}}\'>{{::newsletterSubscription.NewsletterName}}</a>\r\n        </li>\r\n    </ul>\r\n    <span data-ng-if=\'$ctrl.subscriptions && !$ctrl.subscriptions.length\'>{{ \"contactmanagement.contact.newslettersubscriptions.nodata\" | resolve }}</span>\r\n</div>");
$templateCache.put("cms.contactmanagement/contactprofile/journey/journey.component.html","<div class=\'row\'>\r\n    <ul class=\'field-list col-md-4\'>\r\n        <li class=\'row\'>\r\n            <span class=\'field-name col-xs-3 col-md-4\'>{{ \"om.contact.journey.length\" | resolve }}</span>\r\n            <span class=\'col-xs-9 col-md-8 field-value\' data-ng-if=\'$ctrl.journey\'><strong>{{::$ctrl.journey.JourneyLengthDaysText}}</strong> ({{::$ctrl.journey.JourneyLengthStartedDate}})</span>\r\n        </li>\r\n        <li class=\'row\'>\r\n            <span class=\'field-name col-xs-3 col-md-4\'>{{ \"om.contact.journey.lastactivity\" | resolve }}</span>\r\n            <span class=\'col-xs-9 col-md-8 field-value\' data-ng-if=\'$ctrl.journey\'><strong>{{::$ctrl.journey.LastActivityDaysAgoText}}</strong> ({{::$ctrl.journey.LastActivityDate}})</span>\r\n        </li>\r\n    </ul>\r\n</div>");
$templateCache.put("cms.contactmanagement/contactprofile/notes/notes.component.html","<div>\r\n    <div data-ng-bind-html=\'::$ctrl.notes\'></div>\r\n    <span data-ng-if=\'!$ctrl.notes\'>{{ \"contactmanagement.contact.notes.nodata\" | resolve }}</span>\r\n</div>");
$templateCache.put("cms.contactmanagement/contactprofile/persona/persona.component.html","<h4>{{ \"contactmanagement.contact.persona.title\" | resolve }}</h4>\r\n<div class=\'body\'>\r\n    <img alt=\'{{ \"contactmanagement.contact.persona.imagealternativetext\" | resolve }}\' data-ng-src=\'{{::$ctrl.persona.ImageUrl}}\' data-ng-show=\'::$ctrl.persona.ImageUrl\'>\r\n    <div class=\'content\'>\r\n        <h5 data-ng-bind=\'::$ctrl.persona.Name\'></h5>\r\n        <p data-ng-bind=\'::$ctrl.persona.Description\'></p>\r\n    </div>\r\n</div>");
$templateCache.put("cms.contactmanagement/contactprofile/scorings/scorings.component.html","<h4>{{ \"contactmanagement.contact.scoring.title\" | resolve }}</h4>\r\n<div data-ng-if=\'$ctrl.scorings\'>\r\n    <div class=\'row\' data-ng-if=\'$ctrl.scorings.length\'>\r\n        <dl class=\'col-md-3\' data-ng-repeat=\'scoring in $ctrl.scorings track by $index\'>\r\n            <dt>{{::scoring.Points}}</dt>\r\n            <dd>{{::scoring.Name}}</dd>\r\n        </dl>\r\n    </div>\r\n    <span data-ng-if=\'!$ctrl.scorings.length\'>{{ \"contactmanagement.contact.scoring.nodata\" | resolve }}</span>\r\n</div>");
$templateCache.put("cms.contactmanagement/contactprofile/submittedForms/submittedForms.component.html","<div data-ng-if=\'$ctrl.forms\'>\r\n    <table class=\'table table-hover\' data-ng-if=\'$ctrl.forms.length\'>\r\n        <thead>\r\n            <tr class=\'unigrid-head\'>\r\n                <th>\r\n                    <a href=\'#\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeName)\'>{{ \"general.form\" | resolve }}</a>\r\n                    <i class=\'icon-caret-down\' data-ng-show=\'$ctrl.showSorting($ctrl.sortTypeName, $ctrl.sortAsc)\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeName)\'></i>\r\n                    <i class=\'icon-caret-up\' data-ng-show=\'$ctrl.showSorting($ctrl.sortTypeName, !$ctrl.sortAsc)\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeName)\'></i>\r\n                </th>\r\n                <th class=\'site-name-column\'>\r\n                    <a href=\'#\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeSite)\'>{{ \"general.sitename\" | resolve }}</a>\r\n                    <i class=\'icon-caret-down\' data-ng-show=\'$ctrl.showSorting($ctrl.sortTypeSite, $ctrl.sortAsc)\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeSite)\'></i>\r\n                    <i class=\'icon-caret-up\' data-ng-show=\'$ctrl.showSorting($ctrl.sortTypeSite, !$ctrl.sortAsc)\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeSite)\'></i>\r\n                </th>\r\n                <th class=\'submission-date-column\'>\r\n                    <a href=\'#\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeDate)\'>{{ \"contactmanagement.contact.submittedforms.submissiondate\" | resolve }}</a>\r\n                    <i class=\'icon-caret-down\' data-ng-show=\'$ctrl.showSorting($ctrl.sortTypeDate, $ctrl.sortAsc)\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeDate)\'></i>\r\n                    <i class=\'icon-caret-up\' data-ng-show=\'$ctrl.showSorting($ctrl.sortTypeDate, !$ctrl.sortAsc)\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeDate)\'></i>\r\n                </th>\r\n            </tr>\r\n        </thead>\r\n        <tbody class=\'tbody-hover\'>\r\n            <tr data-ng-repeat=\'form in $ctrl.forms | orderBy:$ctrl.sortType:$ctrl.sortAsc\'>\r\n                <td><a target=\'_blank\' data-ng-href=\'{{::form.FormUrl}}\'>{{ ::form.FormDisplayName }}</a></td>\r\n                <td>{{ ::form.SiteDisplayName }}</td>\r\n                <td>{{ ::form.FormSubmissionDate | date:\'medium\' }}</td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n    <span data-ng-if=\'!$ctrl.forms.length\'>{{ \"contactmanagement.contact.submittedforms.nodata\" | resolve }}</span>\r\n</div>");
$templateCache.put("cms.contactmanagement/contactprofile/shared/categoryHeading.component.html","<h4 class=\'anchor\' data-ng-bind=\'$ctrl.heading\'></h4>\r\n<div data-ng-transclude=\'\'></div>");
$templateCache.put("cms.contactmanagement/contactprofile/card/address/address.component.html","<div class=\'tag-group\' data-ng-if=\'::$ctrl.showTags\'>\r\n    <span class=\'tag tag-default\' data-ng-if=\'::$ctrl.contactCard.IsCustomer\'>{{ \"contactmanagement.contact.address.customer\" | resolve }}</span>\r\n    <span class=\'tag tag-default\' data-ng-if=\'::$ctrl.contactCard.IsUser\'>{{ \"contactmanagement.contact.address.user\" | resolve }}</span>\r\n</div>\r\n<ul>\r\n    <li>\r\n        <span class=\'contact-name\' data-ng-if=\'::$ctrl.isMale\'>{{ \"contactmanagement.contact.gender.male\" | resolve }}</span>\r\n        <span class=\'contact-name\' data-ng-if=\'::$ctrl.isFemale\'>{{ \"contactmanagement.contact.gender.female\" | resolve }}</span>\r\n        <span class=\'contact-name\'>{{$ctrl.contactCard.ContactName}}</span>\r\n        <span data-ng-if=\'::$ctrl.contactCard.ContactAge\'>&ndash; {{ \"contactmanagement.contact.age\" | resolve }} {{$ctrl.contactCard.ContactAge}}</span>\r\n    </li>\r\n    <li data-ng-if=\'::$ctrl.contactCard.ContactAddress\'>{{$ctrl.contactCard.ContactAddress}}</li>\r\n    <li>\r\n        <a href=\'mailto:{{$ctrl.contactCard.ContactEmail}}\' data-ng-bind=\'::$ctrl.contactCard.ContactEmail\'></a> \r\n        <cms-card-marketing-email-status contact-id=\'::$ctrl.contactCard.ContactID\' data-ng-if=\'::$ctrl.showMarketingEmailStatus\'></cms-card-marketing-email-status>\r\n    </li>\r\n</ul>");
$templateCache.put("cms.contactmanagement/contactprofile/card/marketingEmailStatus/marketingEmailStatus.component.html","<span class=\'tag-group\' data-ng-switch=\'::$ctrl.marketingEmailStatus\'>\r\n    <span class=\'tag tag-active\' title=\'{{ \"contactmanagement.contact.marketingemailstatus.recievingmarketingemails.desciption\" | resolve }}\' data-ng-switch-when=\'ReceivingMarketingEmails\'>{{ \"contactmanagement.contact.marketingemailstatus.recievingmarketingemails\" | resolve }}</span>\r\n    <span class=\'tag tag-incomplete\' title=\'{{ \"contactmanagement.contact.marketingemailstatus.optedout.desciption\" | resolve }}\' data-ng-switch-when=\'OptedOut\'>{{ \"contactmanagement.contact.marketingemailstatus.optedout\" | resolve }}</span>\r\n    <span class=\'tag tag-incomplete\' title=\'{{ \"contactmanagement.contact.marketingemailstatus.undeliverable.desciption\" | resolve }}\' data-ng-switch-when=\'Undeliverable\'>{{ \"contactmanagement.contact.marketingemailstatus.undeliverable\" | resolve }}</span>\r\n</span>");
$templateCache.put("cms.contactmanagement/contactprofile/detail/accounts/accounts.component.html","<ul class=\'row field-account\' data-ng-if=\'::$ctrl.field.FieldValue.length > 1\'>\r\n    <li on=\'subfield.Type\' class=\'col-md-12\' data-ng-repeat=\'subfield in ::$ctrl.field.FieldValue\' data-ng-switch=\'\'>\r\n        <cms-account account=\'subfield\'></cms-account>\r\n    </li>\r\n</ul>\r\n<cms-account account=\'$ctrl.field.FieldValue[0]\' data-ng-if=\'::$ctrl.field.FieldValue.length === 1\'></cms-account>");
$templateCache.put("cms.contactmanagement/contactprofile/detail/campaign/campaign.component.html","<a href=\'{{::$ctrl.field.Url}}\' target=\'_blank\'>{{::$ctrl.field.Text}}</a>");
$templateCache.put("cms.contactmanagement/contactprofile/detail/accounts/account/account.component.html","<a href=\'{{::$ctrl.account.Url}}\' target=\'_blank\'>{{::$ctrl.account.Text}}</a>\r\n<span data-ng-if=\'::!!$ctrl.account.ContactRole\'> ({{::$ctrl.account.ContactRole}})</span>");}]);return 'cms.contactmanagement/contactprofile/';}})