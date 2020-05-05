cmsdefine([], function () {
    var Controller = function ($scope) {
            this.$scope = $scope;
            this.$scope.onChange = this.onChange.bind(this);
            this.$scope.getValue = this.getValue;
            this.$scope.displayError = this.displayError;

            if (this.$scope.patternError) {
                this.$scope.patternError = 'general.requiresvalue';
            }
        },
        directive = function () {
            return {
                restrict: "E",
                template:
'<div data-ng-class="{\'form-group\': true, \'has-error\': (form.$submitted || model.$dirty) && model.$invalid}" ' +
     'title="{{title}}" >' +
    '<div class="editing-form-label-cell">' +
        '<label for="{{id}}" class="control-label editing-form-label">{{label}}:<span class="required-mark" data-ng-if="required">*</span></label>' +
    '</div>' +
    '<div class="editing-form-value-cell">' +
        '<div class="editing-form-control-nested-control">' +
            '<input type="text" ' +
                    'class="form-control" ' +
                    'data-ng-change="onChange()" ' +
                    'data-ng-model="value" ' +
                    'name="{{id}}" ' +
                    'id="{{id}}" ' +
                    'placeholder="{{placeholder}}" ' +
                    'data-ng-pattern="getValue(pattern)" ' +
                    'data-ng-required="required" ' +
                    'data-ng-maxlength="maxlength" ' +
                    '/>' +
            '<span class="form-control-error" data-ng-if="(form.$submitted || model.$dirty) && model.$error.required">' +
                '{{"general.requiresvalue"|resolve}}' +
            '</span>' +
            '<span class="form-control-error" data-ng-if="displayError(pattern, form.$submitted, model.$dirty) && model.$error.pattern">' +
                '{{patternError|resolve}}' +
            '</span>' +
            '<div class="explanation-text" data-ng-if="explanationText">{{explanationText}}</div>' +
        '</div>' +
    '</div>' +
'</div>',
                replace: true,
                require: "^form",
                scope: {
                    value: '=',
                    required: "=",
                    maxlength: "@",
                    pattern: "=",
                    id: "@",
                    placeholder: '@',
                    title: "@",
                    label: "@",
                    inputType: "@",
                    explanationText: "@",
                    patternError: '@',
                },
                controller: Controller,
                link: function ($scope, $element, $attrs, form) {
                    $scope.form = form;
                    $scope.model = form[$scope.id];
                    if ($attrs.maxlength) {
                        $element.find("input").attr("maxlength", $attrs.maxlength);
                    }
                }
            };
        };


    /**
    * Determines whether the validation error for given errorObject should be displayed. If error object is JavaScript object,
    * checks the content of the display property and display the error depending on the value. Otherwise, returns true
    * if either isSubmitted or isDirty parameter is true.
    * 
    * @param   errorObject     object       object defined on local scope specifying validation expression
    * @param   isSubmitted     boolean      determines whether the input form is in submitted state
    * @param   isDirty         boolean      determines whether the input has been changed by the visitor
    * @returns                 boolean      true, if the validation error should be displayed; otherwise, false
    */
    Controller.prototype.displayError = function(errorObject, isSubmitted, isDirty) {
        if (_.isObject(errorObject)) {
            if (errorObject.display === "afterSubmission") {
                return isSubmitted;
            }
        }

        return isSubmitted || isDirty;
    };


    /**
     * Checks given error object. If the object is JavaScript object, returns content of its value property. 
     * Otherwise, returns the error object itself. This method ensures both string expressions and simple objects can be used
     * as validation expression.
     * 
     * @param   errorObject     object     object defined on local scope specifying validation expression
     * @returns                 string     if errorObject is JavaScript object, returns content of its value property; otherwise, returns errorObject itself
     */
    Controller.prototype.getValue = function(errorObject) {
        if (_.isObject(errorObject)) {
            return errorObject.value;
        }

        return errorObject;
    };
    

    /**
     * Keeps the value of input in correct field.
     */
    Controller.prototype.onChange = function () {
        this.$scope.value = this.$scope.model.$viewValue;
    };

    Controller.$inject = [
      '$scope'
    ];

    return [directive];
});