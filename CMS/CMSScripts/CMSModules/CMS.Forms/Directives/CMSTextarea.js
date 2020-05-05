cmsdefine([], function () {
    
    var Controller = function ($scope) {
        this.$scope = $scope;
        this.$scope.onChange = this.onChange.bind(this);
    },
        directive = function () {
            return {
                restrict: "E",
                template:
    '<div data-ng-class="{\'form-group\': true, \'has-error\': (form.$submitted || model.$dirty) && model.$invalid}" title="{{title}}" >' +
    '<div class="editing-form-label-cell">' +
        '<label for="{{id}}" class="control-label editing-form-label">{{label}}:<span class="required-mark" data-ng-if="required">*</span></label>' +
    '</div>' +
    '<div class="textarea editing-form-value-cell">' +
        '<textarea ' +
                'class="form-control" ' +
                'data-ng-change="onChange()" ' +
                'data-ng-model="value" ' +
                'name="{{id}}" ' +
                'data-ng-readonly="readonly" ' +
                'data-ng-required="required" ' +
                'placeholder="{{placeholder}}" ' +
                'data-ng-maxLength="maxlength" />' +
        '<span class="form-control-error" data-ng-if="(form.$submitted || model.$dirty) && model.$error.required">' +
            '{{"general.requiresvalue"|resolve}}' +
        '</span>' +
        '<span class="form-control-error" data-ng-if="(form.$submitted || model.$dirty) && model.$error.regexPattern">' +
            '{{regexPatternError|resolve}}' +
        '</span>' +
        '<div class="explanation-text" data-ng-if="explanationText">{{explanationText}}</div>' +
    '</div>' +
'</div>',
                replace: true,
                require: ["^form"],
                scope: {
                    value: '=',
                    required: "=",
                    maxlength: "@",
                    id: "@",
                    title: "@",
                    label: "@",
                    inputType: "@",
                    readonly: "=",
                    explanationText: "@",
                    regexPattern: "@",
                    regexPatternModifiers: "@",
                    regexPatternError: "@",
                    placeholder: "@",
                },
                controller: Controller,
                link: function ($scope, $element, $attrs, $ctrl) {
                    $scope.form = $ctrl[0];
                    $scope.model = $scope.form[$scope.id];

                    var $textArea = $element.find("textarea");
                    if ($attrs.rows) {
                        $textArea.attr("rows", $attrs.rows);
                    }
                    
                    // Regex validation
                    $scope.model.$validators.regexPattern = function (modelValue, viewValue) {
                        var value = modelValue || viewValue;

                        var pattern = $attrs.regexPattern || "";
                        var flags = $attrs.regexPatternModifiers || "";

                        var reg = new RegExp(pattern, flags);
                        if (!value || 0 === value.length || reg.test(value)) {
                            return true;
                        }

                        return false;
                    };
                }
            };
        };


     /**
     * Keeps the value of input in required format.
     */
    Controller.prototype.onChange = function () {
        var value = this.$scope.model.$viewValue;

        if (this.$scope.maxlength) {
            value = value.substring(0, this.$scope.maxlength);;
        }

        this.$scope.value = value;
    };

    Controller.$inject = [
      '$scope'
    ];

    return [directive];
});