cmsdefine(["CMS/EventHub", "Underscore"], function (EventHub, _) {

    /** 
     * Directive is responsible for checking changes of the underneath model. When some changes were performed and the model is valid,
     * calls defined callback for performing auto-save.
     *
     * @throws      no form object was found in the ancestor tree
     */

    var Controller = function ($scope, resolveFilter) {
        this.$scope = $scope;
        this.resolveFilter = resolveFilter;

        // Stores contexts from which the error message was displayed in order to avoid showing confirmation message until all errors are resolved.
        this.messageLocks = [];

        // Stores all the requests that are pending due to form invalidity.
        $scope.pending = {};

        // Stores all save promises for autosaves in order to cancel them if needed.
        $scope.savePromise = {};

        // Stores all watches so that we can stop them when requested.
        $scope.watches = {};
        EventHub.subscribe('registerAutosaveContext', function (context, autosaveMethod) {
            // Watch has to be set on the parent scope because parent's scope is bound to the input elements
            $scope.watches[context] = $scope.$watch(context, function (newModel, oldModel) {
                if ($scope.$formController.$dirty) {
                    if ($scope.savePromise[context]) {
                        $scope.$timeout.cancel($scope.savePromise[context]);
                    }

                    $scope.savePromise[context] = $scope.$timeout(function () {
                        $scope.savePromise[context] = null;

                        // Validity could be violated since the last check, make sure it still persists
                        if ($scope.$formController.$valid) {
                            $scope.$formController.$setPristine();

                            // Send all the requests
                            _.each($scope.pending, function (method, pendingContext) {
                                if (context != pendingContext) {
                                    method();
                                }
                                delete $scope.pending[pendingContext];
                                $scope.showInfo($scope.resolveFilter("autosave.pending"), pendingContext);
                            });
                            autosaveMethod(newModel, oldModel);
                        } else {
                            // Invalid form - set request as pending
                            $scope.pending[context] = autosaveMethod.bind(this, newModel, oldModel);
                            $scope.showError($scope.resolveFilter("autosave.validationFailed"), context);
                        }
                    }, 500);
                }
            },
            // Last parameter determines that angular.equal method is used for comparing the values.
            // This causes the watch event is fired for every property of $scope.scopeContext change, not
            // only for the object itself
            true);
        });

        EventHub.subscribe('removeAutosaveContext', function (context) {
            $scope.$timeout.cancel($scope.savePromise[context]);
            this.removeMessageLock(context);
            delete $scope.pending[context];
            $scope.watches[context]();
        }.bind(this));

        $scope.resolveFilter = resolveFilter;
        $scope.showError = this.showError.bind(this);
        $scope.showInfo = this.showInfo.bind(this);
        $scope.hideInfo = this.hideInfo.bind(this);
    };


    /**
     * Displays given text as error in autosave message bar.
     */
    Controller.prototype.showError = function (text, context) {
        if (this.messageLocks.indexOf(context) === -1) {
            this.messageLocks.push(context);
            this.$scope.autosaveStateText = text;
            this.$scope.autosaveClass = "alert-error";
        }
    };


    /**
     * Removes context from autosave message locks.
     */
    Controller.prototype.removeMessageLock = function (context) {
        var index = this.messageLocks.indexOf(context);
        if (index > -1) {
            this.messageLocks.splice(index, 1);
        }
    }


    /**
     * Displays given text as information message in autosave message bar.
     */
    Controller.prototype.showInfo = function (text, context) {
        this.removeMessageLock(context);
        if (this.messageLocks.length === 0) {
            this.$scope.autosaveStateText = text;
            this.$scope.autosaveClass = "alert-info";
        }
    };


    /**
     * Hides autosave message bar.
     */
    Controller.prototype.hideInfo = function (context) {
        this.removeMessageLock(context);
        if (this.messageLocks.length === 0) {
            this.$scope.autosaveClass = "ng-hide";
        }
    };


    return ["$timeout", function ($timeout) {
        return {
            template:
'<form novalidate name={{formName}}>' +
    '<span class="form-autosave-message" ng-class="autosaveClass" data-ng-show="autosaveStateText">{{autosaveStateText}}</span>' +
    '<div data-ng-transclude="" class="form-horizontal"></div>' +
'</form>',
            transclude: true,
            replace: true,
            restrict: "E",
            require: "form",
            controller: ["$scope", "resolveFilter", Controller],
            link: function ($scope, $element, $attrs, $formController) {
                if (!$formController) {
                    throw "Directive has to be child of at least one form.";
                }
                $scope.$root.$broadcast("autosaveForm", $formController);
                $scope.$formController = $formController;
                $scope.$timeout = $timeout;
                $scope.formName = $attrs.formName;

                EventHub.subscribe('savingFailed', function (context, text) {
                    var message = $scope.resolveFilter("autosave.failed");
                    if (_.isString(text)) {
                        message = text;
                    }
                    $scope.showError(message, context);
                });

                EventHub.subscribe('savingFinished', function (context) {
                    $scope.showInfo($scope.resolveFilter("autosave.finished"), context);
                });

                EventHub.subscribe('hideInfo', function (context) {
                    $scope.hideInfo(context);
                });
            }
        };
    }];
});