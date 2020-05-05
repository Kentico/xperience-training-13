cmsdefine([], function () {

    var Controller = function ($scope) {
        this.$scope = $scope;
    },
        directive = function () {
            return {
                restrict: "E",
                template:
'<div data-ng-class="{\'form-group\': true, \'has-error\': !model.$valid}" title="{{title}}">' +
    '<div class="editing-form-label-cell">' +
        '<label data-ng-if="label" for="{{id}}" class="control-label editing-form-label">{{label}}:</label>' +
    '</div>' +
    '<div class="editing-form-value-cell">' +
        '<div class="editing-form-control-nested-control">' +
            '<div class="radio-list-vertical">' +
                '<div class="radio" data-ng-repeat="option in options">' +
                    '<input  id="{{option.id}}" ' +
                            'type="radio" ' +
                            'value="{{option.value}}" ' +
                            'name="{{$parent.name}}" ' +
                            'data-ng-model="$parent.value" />' +
                    '<label for="{{option.id}}">{{option.label | resolve}}</label>' +
                '</div>' +
            '</div>' +
        '</div>' +
    '</div>' +
'</div>',
                replace: true,
                require: "^form",
                scope: {
                    value: "=",
                    id: "@",
                    name: '@',
                    options: '=',
                    title: "@",
                    label: "@",
                },
                controller: Controller,
                link: function ($scope, $element, $attrs, form) {
                    // Provide model attributes to the template (validation)
                    $scope.model = form[$scope.id];
                }
            };
        };


    Controller.$inject = [
      '$scope'
    ];

    return [directive];
});